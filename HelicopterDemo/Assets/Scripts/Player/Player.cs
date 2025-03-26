using System.Collections.Generic;
using UnityEngine;
using static InputDeviceBase;
using static Types;
using static ViewPortController;

[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(LineDrawer))]

public class Player : MonoBehaviour
{
    [SerializeField] private bool startWithTakeoff = false;
    [SerializeField] float changeSpeedInput = 0.7f;
    [SerializeField] float vertFastCoef = 5f;
    [SerializeField] float minDistToAim = 17f;
    [SerializeField] float maxDistToAim = 20f;
    [SerializeField] private float minDistToBuild = 5f;
    [SerializeField] float speed = 20f;
    [SerializeField] float lowSpeedCoef = 0.5f;
    [SerializeField] float highSpeedCoef = 3f;
    [SerializeField] float verticalSpeed = 30f;
    [SerializeField] float lateralMovingCoef = 0.1f;
    [SerializeField] float acceleration = 1f;
    [SerializeField] float delayAfterDestroy = 1f;
    [SerializeField] Health health;
    [SerializeField] GameObject airDustPrefab;
    [SerializeField] GameObject deadPrefab;
    [SerializeField] GameObject explosion;
    [SerializeField] private ScreenFading screenFading;
    [SerializeField] ControllerType controllerType = ControllerType.Keyboard;
    [SerializeField] Players playerNumber = Players.Player1;
    [SerializeField] PlayerStates playerState = PlayerStates.Normal;

    bool rotateToDirection;
    float yawAngle;
    float currVerticalSpeed, targetVerticalSpeed;
    private float currDelayAfterDestroy;
    Vector3 currSpeed, targetSpeed;
    Vector3 targetDirection;
    GameObject possibleTarget, selectedTarget, possiblePlatform, selectedPlatform, lastPlatform;
    Translation translation;
    Rotation rotation;
    CrosshairController crosshairController;
    NpcController npcController;
    PlatformController platformController;
    InputDeviceBase inputDevice;
    InputController inputController;
    Shooter shooter;
    Crosshair crosshair;
    private PlayerBody playerBody;
    private AirDuster airDuster;
    private LineDrawer lineDrawer;
    private List<SimpleRotor> rotors;
    private TakeoffProcess takeoff;
    private RandomMovement randomMovement;

    public bool Aiming { get; private set; }
    public bool StartWithTakeoff => startWithTakeoff;
    public bool TargetDestroy { get; set; }
    public bool IsAlive => health.IsAlive;
    public Players PlayerNumber => playerNumber;
    public Vector3 AimAngles { get; private set; }
    public Vector3 CurrentDirection { get; private set; }
    public InputDeviceBase InputDevice => inputDevice;
    public PlayerBody PlayerBody => playerBody;
    public GameObject DeadPlayer { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        translation = GetComponent<Translation>();
        rotation = GetComponentInChildren<Rotation>();
        playerBody = GetComponentInChildren<PlayerBody>();
        shooter = GetComponent<Shooter>();
        rotors = new List<SimpleRotor>();
        rotors.AddRange(GetComponentsInChildren<SimpleRotor>());
        foreach (var rotor in rotors)
            rotor.StartRotor();
        lineDrawer = GetComponent<LineDrawer>();
        takeoff = GetComponent<TakeoffProcess>();
        randomMovement = GetComponent<RandomMovement>();

        npcController = NpcController.Singleton;
        platformController = PlatformController.Singleton;
        crosshairController = CrosshairController.singleton;
        crosshair = crosshairController.GetCrosshair(playerNumber);

        if (airDustPrefab)
            airDuster = Instantiate(airDustPrefab, transform).GetComponent<AirDuster>();
        airDuster.normRotorSpeed = 1f;

        inputController = InputController.singleton;
        if (!inputController) return;
        inputDevice = inputController.GetDevice(controllerType);

        inputDevice.TryBindingToObject += TryBindingToObject;
        inputDevice.TryLaunchUnguidedMissile += TryLaunchUnguidedMissile;
        inputDevice.CancelBuildSelection += CancelBuildSelection;
        inputDevice.TryLaunchGuidedMissile += TryLaunchGuidedMissile;
        inputDevice.StartSelectionFarTarget += StartSelectionFarTarget;
        inputDevice.StartSelectionAnyTarget += StartSelectionAnyTarget;
        inputDevice.CancelSelectionAnytarget += CancelSelectionAnytarget;
        inputDevice.CancelAiming += CancelAiming;
        inputDevice.SelectBuildingEvent += SelectBuilding;
        inputDevice.TakeEvent += Take;

        rotateToDirection = false;
        targetDirection = transform.forward;
        CurrentDirection = transform.forward;

        //hide cursor in center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputDirection = inputDevice.GetInput();
        float inputVerticalDirection = inputDevice.VerticalMoving;
        float inputVerticalFast = inputDevice.VerticalFastMoving;
        float inputX = inputDirection.x;
        float inputZ = inputDirection.y;
        float inputY = inputVerticalFast != 0f ? inputVerticalFast : inputVerticalDirection;

        if (!health.IsAlive && Respawn())
        {
            health.SetAlive(true);
        }
        else if (startWithTakeoff)
        {
            if (takeoff.Takeoff())
            {
                startWithTakeoff = false;
            }
            else
            {
                Vector3 randMov = new Vector3();
                if (takeoff.ClimbSpeed > 0f)
                    randMov = randomMovement.GetRandomInput();
                Translate(randMov.x, takeoff.ClimbSpeed, randMov.z, 0f);
            }
        }
        else
        {
            //todo: random movement in idle state
            //if (inputX == 0f && inputZ == 0f)
            //{
            //    var randomInput = randomMovement.GetRandomInput();
            //    inputX = randomInput.x;
            //    inputZ = randomInput.z;
            //}
            Translate(inputX, inputY, inputZ, inputVerticalFast);
        }

        //rotation around X, Y, Z
        if (rotation != null)
            Rotate(inputX);

        if (inputDevice.MinigunFire)
            shooter.BarrelFire(selectedTarget);

        if (IsAlive && inputDevice.PlayerState == PlayerStates.Normal)
            DrawLineToTarget();
        else
            lineDrawer.Enabled = false;

        if (inputDevice.PlayerState == PlayerStates.Aiming)
        {
            if (selectedTarget && !possibleTarget)
            {
                var npcGround = selectedTarget.GetComponent<NpcGround>();
                possibleTarget = npcGround ? npcGround.GetNextMember() : possibleTarget;
            }
            else if (!selectedTarget && possibleTarget)
            {
                selectedTarget = possibleTarget;
                possibleTarget = null;
            }
            else if (!selectedTarget)
            {
                TargetDestroy = true;
                inputDevice.ForceChangePlayerState(PlayerStates.Normal);
                ChangeAimState();
            }
        }

        airDuster.normRotorSpeed = 1f;
        airDuster.normAltitiude = transform.position.y / 10f;

        //if (playerNumber == Players.Player1) Debug.Log(health.CurrHp);
        //if (playerNumber == Players.Player1) Debug.Log(inputDevice.PlayerState);
    }

    void Translate(float inputX, float inputY, float inputZ, float inputVerticalFast)
    {
        float inputXZ = Mathf.Clamp01(new Vector3(inputX, 0f, inputZ).magnitude);

        if (inputXZ >= changeSpeedInput && !translation.RotToDir ||
            inputXZ < changeSpeedInput && translation.RotToDir)
            rotateToDirection = translation.SwitchRotation();

        targetDirection = translation.TargetDirectionNorm;

        if (!inputDevice.PlayerCanTranslate)
            inputX = inputY = inputZ = 0f;

        if (inputDevice.PlayerState == PlayerStates.Aiming && selectedTarget)
        {
            Vector3 inputXYZ = new Vector3(inputX, inputY, inputZ);
            inputXYZ = BalanceDistToTarget(inputXYZ);
            targetSpeed = Vector3.ClampMagnitude(inputXYZ * speed * lowSpeedCoef, speed * lowSpeedCoef);
            currSpeed = Vector3.Lerp(currSpeed, targetSpeed, acceleration * Time.deltaTime);

            translation.SetRelToTargetTranslation(currSpeed, yawAngle);
        }
        else
        {
            Vector3 inputXYZ = new Vector3(inputX, inputY, inputZ);
            float bombMovingCoef = playerBody.Item ? lowSpeedCoef : 1f;

            if (inputDevice.FastMoving && !playerBody.Item)
            {
                inputXYZ = new Vector3(inputX, inputY, inputZ);
                targetSpeed = Vector3.ClampMagnitude(inputXYZ * speed * highSpeedCoef, speed * highSpeedCoef);
            }
            else if (inputXZ == 0f)
                targetSpeed = Vector3.zero;
            else
                targetSpeed = Vector3.ClampMagnitude(inputXYZ * speed * bombMovingCoef, speed * bombMovingCoef);

            currSpeed = Vector3.Lerp(currSpeed, targetSpeed, acceleration * Time.deltaTime);
            translation.SetHorizontalTranslation(currSpeed);
        }

        targetVerticalSpeed = inputY * verticalSpeed;
        if (inputVerticalFast != 0f) targetVerticalSpeed *= vertFastCoef;
        currVerticalSpeed = Mathf.Lerp(currVerticalSpeed, targetVerticalSpeed, acceleration * Time.deltaTime);
        translation.SetVerticalTranslation(currVerticalSpeed);
    }

    void Rotate(float inputX)
    {
        CurrentDirection = rotation.CurrentDirection;
        AimAngles = rotation.AimAngles;
        yawAngle = rotation.YawAngle;

        if (inputDevice.PlayerState == PlayerStates.Aiming && selectedTarget)
        {
            Quaternion rotToTarget = Quaternion.LookRotation((selectedTarget.transform.position - this.transform.position));
            rotation.RotateToTarget(rotToTarget, inputX);
        }
        else if (inputDevice.AimMovement)
        {
            var direction = crosshair.HitPoint - transform.position;
            rotation.RotateToDirection(direction, 0f, true);
        }
        else
        {
            var direction = targetDirection != Vector3.zero ? targetDirection : CurrentDirection;
            var speedCoef = targetDirection != Vector3.zero ? currSpeed.magnitude / speed : 0f;
            rotation.RotateToDirection(direction, speedCoef, rotateToDirection);
        }
    }

    void ChangeAimState()
    {
        Aiming = !Aiming;
        selectedTarget = Aiming ? possibleTarget : null;
        if (Aiming) lineDrawer.Enabled = false;
    }

    void DrawLineToTarget()
    {
        KeyValuePair<GameObject, float> nearest;
        TargetTypes targetType;
        var nearestNpc = npcController != null ? npcController.FindNearestEnemy(transform.position) : new KeyValuePair<GameObject, float>(null, Mathf.Infinity);
        var nearestPlatform = platformController != null ? platformController.FindNearestPlatform(transform.position) : new KeyValuePair<GameObject, float>(null, Mathf.Infinity);

        nearest = nearestNpc.Value < nearestPlatform.Value ? nearestNpc : nearestPlatform;
        targetType = nearestNpc.Value < nearestPlatform.Value ? TargetTypes.Enemy : TargetTypes.Platform;

        if (nearest.Key)
        {
            var aimOrigin = nearest.Key.GetComponentInChildren<AimOrigin>();
            if (nearest.Value < minDistToAim && targetType == TargetTypes.Enemy)
            {
                lineDrawer.Enabled = true;
                Color lineColor = Color.red;
                lineDrawer.SetColor(lineColor);
                lineDrawer.SetPosition(transform.position, aimOrigin ? aimOrigin.gameObject.transform.position : nearest.Key.transform.position);
                possibleTarget = nearest.Key;
                possiblePlatform = null;
            }
            else if (nearest.Value < minDistToBuild && targetType == TargetTypes.Platform)
            {
                lineDrawer.Enabled = true;
                Color lineColor = Color.blue;
                lineDrawer.SetColor(lineColor);
                lineDrawer.SetPosition(transform.position, aimOrigin ? aimOrigin.gameObject.transform.position : nearest.Key.transform.position);
                possibleTarget = null;
                possiblePlatform = nearest.Key;
                possiblePlatform.GetComponent<Platform>().ShowPlatform();
                lastPlatform = possiblePlatform;
            }
            else
            {
                if (lastPlatform)
                {
                    var platform = lastPlatform.GetComponent<Platform>();
                    if (!platform.IsReserved) platform.HidePlatform();
                }

                lineDrawer.Enabled = false;
                possibleTarget = possiblePlatform = null;
            }
        }
    }

    Vector3 BalanceDistToTarget(Vector3 input)
    {
        if (input.z == 0f)
            input.z = Mathf.Abs(input.x) * lateralMovingCoef;
        return input;
    }

    PlayerStates TryBindingToObject(PlayerStates playerState)
    {
        if (possibleTarget)
        {
            ChangeAimState();
            possibleTarget = null;
            return PlayerStates.Aiming;
        }
        else if (possiblePlatform)
        {
            StartBuildSelection();
            return PlayerStates.BuildSelection;
        }
        else
        {
            TryLaunchUnguidedMissile();
            return playerState;
        }
    }

    void TryLaunchUnguidedMissile() => shooter.UnguidedMissileLaunch(selectedTarget);

    void StartBuildSelection()
    {
        selectedPlatform = possiblePlatform;
        lineDrawer.Enabled = false;
    }

    void CancelBuildSelection() => selectedPlatform = null;

    void TryLaunchGuidedMissile()
    {
        if (crosshair.SelectedTarget) shooter.GuidedMissileLaunch(crosshair.SelectedTarget);
        crosshair.Hide();
    }

    void StartSelectionFarTarget() => crosshair.Show();

    void StartSelectionAnyTarget() => crosshair.Show();

    void CancelSelectionAnytarget() => crosshair.Hide();

    void CancelAiming() => ChangeAimState();

    private void SelectBuilding(int buildNumber, GlobalSide2 side)
    {
        if (selectedPlatform)
            selectedPlatform.GetComponent<BuildingSelector>().CallBuilding(buildNumber, side);
    }

    private bool Respawn()
    {
        inputDevice.ForceChangePlayerState(PlayerStates.Normal);
        if (Aiming) ChangeAimState();

        if (currDelayAfterDestroy > delayAfterDestroy)
        {
            health.gameObject.SetActive(true);
            transform.position = new Vector3(0, 10, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);

            currDelayAfterDestroy = 0f;
            return true;
        }
        else
        {
            if (currDelayAfterDestroy > 2f / 3f * delayAfterDestroy)
                screenFading.StartFading();

            if (health.gameObject.activeSelf)
            {
                health.gameObject.SetActive(false);
                if (deadPrefab) DeadPlayer = Instantiate(deadPrefab, transform.position, health.gameObject.transform.rotation);
                if (explosion) Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
            }

            currDelayAfterDestroy += Time.deltaTime;
            return false;
        }
    }

    private void Take()
    {
        if (playerBody.ItemForTake && !playerBody.Item)
            playerBody.Take();
        else if (playerBody.Item)
            playerBody.Drop();
    }

    public enum TargetTypes
    {
        None,
        Enemy,
        Platform
    }
}