using System.Collections.Generic;
using UnityEngine;
using static InputDeviceBase;
using static ViewPortController;

[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(LineDrawer))]

public class Player : MonoBehaviour
{
    [SerializeField] float changeSpeedInput = 0.7f;
    [SerializeField] float vertFastCoef = 5f;
    [SerializeField] float minDistToAim = 17f;
    [SerializeField] float maxDistToAim = 20f;
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
    [SerializeField] ControllerType controllerType = ControllerType.Keyboard;
    [SerializeField] Players playerNumber = Players.Player1;

    bool rotateToDirection;
    float yawAngle;
    float currVerticalSpeed, targetVerticalSpeed;
    private float currDelayAfterDestroy;
    Vector3 currSpeed, targetSpeed;
    Vector3 targetDirection;
    GameObject possibleTarget, selectedTarget, possiblePlatform, selectedPlatform;
    Translation translation;
    Rotation rotation;
    CrosshairController crosshairController;
    NpcController npcController;
    PlatformController platformController;
    InputDeviceBase inputDevice;
    InputController inputController;
    Shooter shooter;
    Crosshair crosshair;
    private AirDuster airDuster;
    private LineDrawer lineDrawer;
    private List<SimpleRotor> rotors;

    public bool Aiming { get; private set; }
    public bool TargetDestroy { get; set; }
    public Players PlayerNumber => playerNumber;
    public Vector3 AimAngles { get; private set; }
    public Vector3 CurrentDirection { get; private set; }
    public InputDeviceBase InputDevice => inputDevice;

    // Start is called before the first frame update
    void Start()
    {
        translation = GetComponent<Translation>();
        rotation = GetComponentInChildren<Rotation>();
        shooter = GetComponent<Shooter>();
        rotors = new List<SimpleRotor>();
        rotors.AddRange(GetComponentsInChildren<SimpleRotor>());
        foreach (var rotor in rotors)
            rotor.StartRotor();
        lineDrawer = GetComponent<LineDrawer>();

        npcController = NpcController.singleton;
        platformController = PlatformController.singleton;
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
        float inputX = inputDirection.x;
        float inputZ = inputDirection.y;

        if (!health.IsAlive && Respawn())
        {
            health.SetAlive(true);
        }
        else
            Translate(inputX, inputZ);

        //rotation around X, Y, Z
        if (rotation != null)
            Rotate(inputX);

        if (inputDevice.MinigunFire)
            shooter.BarrelFire(selectedTarget);

        if (inputDevice.PlayerState == PlayerStates.Normal)
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
    }

    void Translate(float inputX, float inputZ)
    {
        float inputVerticalDirection = inputDevice.VerticalMoving;
        float inputVerticalFast = inputDevice.VerticalFastMoving;

        float inputXZ = Mathf.Clamp01(new Vector3(inputX, 0f, inputZ).magnitude);
        float inputY = inputVerticalFast != 0f ? inputVerticalFast : inputVerticalDirection;

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

            if (inputDevice.FastMoving)
            {
                inputXYZ = new Vector3(inputX, inputY, inputZ);
                targetSpeed = Vector3.ClampMagnitude(inputXYZ * speed * highSpeedCoef, speed * highSpeedCoef);
            }
            else if (inputXZ == 0f)
                targetSpeed = Vector3.zero;
            else
                targetSpeed = Vector3.ClampMagnitude(inputXYZ * speed, speed);

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
        var nearestNpc = npcController ? npcController.FindNearestEnemy(transform.position) : new KeyValuePair<GameObject, float>(null, Mathf.Infinity);
        var nearestPlatform = platformController ? platformController.FindNearestPlatform(transform.position) : new KeyValuePair<GameObject, float>(null, Mathf.Infinity);

        nearest = nearestNpc.Value < nearestPlatform.Value ? nearestNpc : nearestPlatform;
        targetType = nearestNpc.Value < nearestPlatform.Value ? TargetTypes.Enemy : TargetTypes.Platform;

        if (nearest.Key)
        {
            var aimOrigin = nearest.Key.GetComponentInChildren<AimOrigin>();
            if (nearest.Value < minDistToAim)
            {
                lineDrawer.Enabled = true;
                Color lineColor = targetType == TargetTypes.Enemy ? Color.red : Color.blue;
                lineDrawer.SetColor(lineColor);
                lineDrawer.SetPosition(transform.position, aimOrigin ? aimOrigin.gameObject.transform.position : nearest.Key.transform.position);
                possibleTarget = targetType == TargetTypes.Enemy ? nearest.Key : null;
                possiblePlatform = targetType == TargetTypes.Platform ? nearest.Key : null;
            }
            else
            {
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
            if (health.gameObject.activeSelf)
            {
                health.gameObject.SetActive(false);
                if (deadPrefab) Instantiate(deadPrefab, transform.position, health.gameObject.transform.rotation);
                if (explosion) Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
            }

            currDelayAfterDestroy += Time.deltaTime;
            return false;
        }
    }

    public enum Axis_Proto : int
    { X, Y, Z }

    public enum TargetTypes
    {
        None,
        Enemy,
        Platform
    }
}
