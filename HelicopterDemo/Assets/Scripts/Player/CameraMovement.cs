using UnityEngine;
using static ViewPortController;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private bool twoShoulders = false;
    [SerializeField] private float defaultVerticalAngle = 15f;
    [SerializeField] private float maxHorizontalAngle = 25f;
    [SerializeField] private float maxVerticalAngle_cameraUp = 40f;
    [SerializeField] private float maxVerticalAngle_cameraDown = 5f;
    [SerializeField] private float rotSpeed = 0.5f;
    [SerializeField] private float rotSpeedManual = 1f;
    [SerializeField] private float aimingSpeed = 6f;
    [SerializeField] private float maxDelay = 0.5f;

    [Header("Camera positions & rotations")]
    [SerializeField] private Vector3 cameraDefaultPos = new Vector3(0, 5, -11);
    [SerializeField] private Vector3 cameraDefaultRot = new Vector3(15, 0, 0); //unused
    [SerializeField] private Vector3 cameraAimPosCenter = new Vector3(1.5f, 1.9f, -4.3f);
    [SerializeField] private Vector3 cameraAimPosRight = new Vector3(1.5f, 1.9f, -4.3f);
    [SerializeField] private Vector3 cameraAimingRot = new Vector3(3.3f, 0, 0);
    [SerializeField] private Vector3 cameraTgtSelPos = new Vector3(0, 11, -22);
    [SerializeField] private Vector3 cameraTakeoffPos = new Vector3(0, 5f, -10f);

    [SerializeField] private Player player;
    [SerializeField] private GameObject cameraContainer;

    private bool delayAfterTargetDestroy;
    private float delay;
    private float currAimingSpeed;
    private Vector2 input, direction, playerInput;
    private Vector3 cameraAimPosLeft;
    private Vector3 cameraAimPos;
    private Crosshair crosshair;
    private ViewPortController viewPortController;
    private Camera playerCamera;
    private CrosshairController crosshairController;

    public bool CameraInTakeoff { get; set; }
    public bool MoveCamera { get; set; }
    public float CameraSpeedInTakeoff { get; set; }

    private bool Aiming => player.Aiming;
    private Vector3 AimAngles => player.AimAngles;
    private Vector3 PlayerDir => player.CurrentDirection;
    private InputDeviceBase inputDevice => player.InputDevice;

    private void Start()
    {
        playerCamera = GetComponent<Camera>();

        viewPortController = ViewPortController.singleton;
        switch(player.PlayerNumber)
        {
            case Players.Player2:
                viewPortController.SetCameraPlayer2(playerCamera);
                break;
            default:
                viewPortController.SetCameraPlayer1(playerCamera);
                break;
        }

        crosshairController = CrosshairController.singleton;
        crosshairController.SetCamera(playerCamera, player.PlayerNumber);
        crosshair = crosshairController.GetCrosshair(player.PlayerNumber);

        cameraAimPos = cameraAimPosRight;
        CameraInTakeoff = player.StartWithTakeoff;
        if (CameraInTakeoff)
        {
            transform.localPosition = player.transform.position + cameraTakeoffPos;
            transform.LookAt(player.transform);
        }
    }

    private void Update()
    {
        input = inputDevice.GetCameraInput();
        playerInput = inputDevice.GetInput();

        Vector2 toTargetSelection = new Vector2();
        if (inputDevice.AimMovement)
        {
            crosshair.Translate(playerInput);
            toTargetSelection = crosshair.ToTargetSelection;
        }
        direction = new Vector2(inputDevice.AimMovement ? toTargetSelection.x : PlayerDir.x,
            inputDevice.AimMovement ? toTargetSelection.y : 0f);

        if (CameraInTakeoff)
        {
            if (MoveCamera)
            {
                RotateHorizontally();
                RotateVertically();
            }
            SetDefault();
            currAimingSpeed = CameraSpeedInTakeoff;
        }
        else if (!Aiming)
        {
            if (IsDelayAfterTargetDestroy())
                currAimingSpeed = 0f;
            else
                currAimingSpeed = aimingSpeed / 2f;

            RotateHorizontally();
            RotateVertically();
            SetDefault();
        }
        else
            RotateWithPlayer();
    }

    private void RotateHorizontally()
    {
        float playerDirX = direction.x;
        float inputHor = input.x;
        playerDirX += inputHor;

        float targetCameraHorRot = playerDirX * maxHorizontalAngle;

        Vector3 eulerAnglesCurrent = transform.localRotation.eulerAngles;
        float currRotSpeed = inputHor != 0f ? rotSpeedManual : rotSpeed;
        Vector3 eulerAnglesTarget = new Vector3(eulerAnglesCurrent.x, targetCameraHorRot, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationTarget, currRotSpeed * Time.deltaTime);
    }

    private void RotateVertically()
    {
        float playerDirZ = direction.y;
        float inputVert = input.y;
        playerDirZ -= inputVert;

        float targetCameraVertRot;
        if (playerDirZ > 0f)
            targetCameraVertRot = playerDirZ * maxVerticalAngle_cameraUp;
        else if (playerDirZ < 0f)
            targetCameraVertRot = playerDirZ * maxVerticalAngle_cameraDown;
        else
            targetCameraVertRot = defaultVerticalAngle;

        Vector3 eulerAnglesCurrent = transform.localRotation.eulerAngles;
        float currRotSpeed = rotSpeedManual;
        Vector3 eulerAnglesTarget = new Vector3(targetCameraVertRot, eulerAnglesCurrent.y, eulerAnglesCurrent.z);

        Quaternion rotationTarget = Quaternion.Euler(eulerAnglesTarget);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationTarget, currRotSpeed * Time.deltaTime);
    }

    private void RotateWithPlayer()
    {
        cameraContainer.transform.rotation = Quaternion.Lerp(cameraContainer.transform.rotation, Quaternion.Euler(AimAngles), aimingSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(cameraAimingRot), aimingSpeed * Time.deltaTime);

        if (twoShoulders)
        {
            if (playerInput.x > 0f)
                cameraAimPos = cameraAimPosRight;
            else if (playerInput.x < 0f)
            {
                cameraAimPosLeft = new Vector3(-cameraAimPosRight.x, cameraAimPosRight.y, cameraAimPosRight.z);
                cameraAimPos = cameraAimPosLeft;
            }
            else
                cameraAimPos = cameraAimPosCenter;
        }
        else
            cameraAimPos = cameraAimPosRight;
        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraAimPos, aimingSpeed * Time.deltaTime);
    }

    private void SetDefault()
    {
        Vector3 cameraPos = inputDevice.AimMovement ? cameraTgtSelPos : cameraDefaultPos;
        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraPos, currAimingSpeed * Time.deltaTime);
        cameraContainer.transform.rotation = Quaternion.Lerp(cameraContainer.transform.rotation, Quaternion.Euler(0f, 0f, 0f), currAimingSpeed * Time.deltaTime);
    }

    private bool IsDelayAfterTargetDestroy()
    {
        if (player.TargetDestroy)
        {
            delayAfterTargetDestroy = true;
            player.TargetDestroy = false;
        }

        if (delayAfterTargetDestroy)
        {
            delay += Time.deltaTime;

            if (delay >= maxDelay)
            {
                delayAfterTargetDestroy = false;
                delay = 0f;
                return false;
            }
            else
                return true;
        }
        else
            return false;
    }
}
