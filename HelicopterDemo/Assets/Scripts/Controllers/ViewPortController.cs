using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class ViewPortController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float deltaRounding = 0.01f;
    [SerializeField] private float dividingLineWidth = 0.01f;
    [SerializeField] private float defaultFov = 60f;
    [SerializeField] private float verticalFov = 70f;
    [SerializeField] private float horizontalFov = 35f;
    [SerializeField] private CamerasConfig camerasConfig = CamerasConfig.player1_player2;
    [SerializeField] private Orientation orientation = Orientation.Vertical;
    [SerializeField] private CameraPosition posCamera1;
    [SerializeField] private CameraPosition posCamera2;

    private bool animForConnect, animForDisconnect;
    private Vector3 divPoint1, divPoint2;
    private Camera cameraPlayer1, cameraPlayer2;
    private InputController inputController;
    private InputDeviceBase inputDevice;
    private LineRenderer dividingLine;

    private CameraSize SizeCamera1 => GetCameraSize(posCamera1);
    private CameraSize SizeCamera2 => GetCameraSize(posCamera2);

    public static ViewPortController singleton { get; private set; }

    private void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!(SizeCamera1 == CameraSize.Full && SizeCamera2 == CameraSize.None ||
            SizeCamera1 == CameraSize.None && SizeCamera2 == CameraSize.Full ||
            SizeCamera1 == CameraSize.Half && SizeCamera2 == CameraSize.Half))
        {
            Debug.LogError("Invalid ViewPortController Start");
        }

        inputController = InputController.singleton;
        inputDevice = inputController.GetInputCommon;
        inputDevice.ChangePlayerConnection += SwitchConnection;
        inputDevice.ChangeConfiguration += SwitchConfiguration;
        inputDevice.ChangeOrientation += SwitchOrientation;

        dividingLine = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Rect rect1 = GetRect(posCamera1);
        Rect rect2 = GetRect(posCamera2);
        Rect currRect1 = rect1, currRect2 = rect2;

        if (cameraPlayer1 && cameraPlayer2)
        {
            if (animForConnect)
            {
                currRect1 = LerpRect(cameraPlayer1.rect, rect1, speed * Time.deltaTime);
                currRect2 = LerpRect(cameraPlayer2.rect, rect2, speed * Time.deltaTime);
                cameraPlayer1.enabled = true;
                cameraPlayer2.enabled = true;

                if (IsViewportHalf(cameraPlayer1.rect) && IsViewportHalf(cameraPlayer2.rect))
                {
                    animForConnect = false;

                    currRect1 = rect1;
                    currRect2 = rect2;
                }
            }

            if (animForDisconnect)
            {
                currRect1 = LerpRect(cameraPlayer1.rect, rect1, speed * Time.deltaTime);
                currRect2 = LerpRect(cameraPlayer2.rect, rect2, speed * Time.deltaTime);
                cameraPlayer1.enabled = true;
                cameraPlayer2.enabled = true;

                if (!IsViewportEnabled(cameraPlayer1.rect) || !IsViewportEnabled(cameraPlayer2.rect))
                {
                    animForDisconnect = false;

                    currRect1 = rect1;
                    currRect2 = rect2;
                    cameraPlayer1.enabled = IsViewportEnabled(cameraPlayer1.rect);
                    cameraPlayer2.enabled = IsViewportEnabled(cameraPlayer2.rect);
                }
            }

            cameraPlayer1.rect = currRect1;
            cameraPlayer2.rect = currRect2;
        }
    }

    private void LateUpdate()
    {
        SetDividingLine();
        if (dividingLine.enabled) DrawDividingLine();
    }

    public void SetCameraPlayer1(Camera cam)
    {
        cameraPlayer1 = cam;
        cameraPlayer1.rect = GetRect(posCamera1);
        cameraPlayer1.enabled = IsViewportEnabled(cameraPlayer1.rect);
    }
    public void SetCameraPlayer2(Camera cam)
    {
        cameraPlayer2 = cam;
        cameraPlayer2.rect = GetRect(posCamera2);
        cameraPlayer2.enabled = IsViewportEnabled(cameraPlayer2.rect);
    }
    public void SwitchConfiguration()
    {
        if (camerasConfig == CamerasConfig.player1_player2)
            camerasConfig = CamerasConfig.player2_player1;
        else
            camerasConfig = CamerasConfig.player1_player2;

        if (SizeCamera1 == CameraSize.Full && SizeCamera2 == CameraSize.None ||
            SizeCamera1 == CameraSize.None && SizeCamera2 == CameraSize.Full ||
            SizeCamera1 == CameraSize.Half && SizeCamera2 == CameraSize.Half)
        {
            posCamera1 = ChangePosByConfiguration(posCamera1);
            posCamera2 = ChangePosByConfiguration(posCamera2);
        }
        else
        {
            Debug.LogError("Invalid ViewPortController SwitchConfiguration");
        }
    }

    public void SwitchOrientation()
    {
        if (orientation == Orientation.Vertical)
            orientation = Orientation.Horizontal;
        else
            orientation = Orientation.Vertical;

        posCamera1 = ChangePosByOrientation(posCamera1);
        posCamera2 = ChangePosByOrientation(posCamera2);
    }

    public void SwitchConnection(int playerNumber)
    {
        if (playerNumber == 1 && SizeCamera1 == CameraSize.None ||
            playerNumber == 2 && SizeCamera2 == CameraSize.None)
        {
            posCamera1 = ChangePosByConnection(posCamera1);
            posCamera2 = ChangePosByConnection(posCamera2);
            animForConnect = true;
        }
        else if (playerNumber == 1 && SizeCamera1 == CameraSize.Half)
        {
            posCamera1 = ChangePosToEmptyByDisconnect(posCamera1);
            posCamera2 = ChangePosToFullByDisconnect(posCamera2);
            animForDisconnect = true;
        }
        else if (playerNumber == 2 && SizeCamera2 == CameraSize.Half)
        {
            posCamera1 = ChangePosToFullByDisconnect(posCamera1);
            posCamera2 = ChangePosToEmptyByDisconnect(posCamera2);
            animForDisconnect = true;
        }
        else
        {
            Debug.LogError("ViewPortController: Invalid Player Connection Change");
        }
    }

    private Rect GetRect(CameraPosition pos)
    {
        switch (pos)
        {
            case CameraPosition.FullLeft:
            case CameraPosition.FullRight:
            case CameraPosition.FullUp:
            case CameraPosition.FullDown:
                return new Rect(0f, 0f, 1f, 1f);
            case CameraPosition.HalfLeft: return new Rect(0f, 0f, 0.5f, 1f);
            case CameraPosition.HalfRight: return new Rect(0.5f, 0f, 0.5f, 1f);
            case CameraPosition.HalfUp: return new Rect(0f, 0f, 1f, 0.5f);
            case CameraPosition.HalfDown: return new Rect(0f, 0.5f, 1f, 0.5f);
            case CameraPosition.EmptyLeft: return new Rect(0f, 0f, 0f, 1f);
            case CameraPosition.EmptyRight: return new Rect(1f, 0f, 0f, 1f);
            case CameraPosition.EmptyUp: return new Rect(0f, 0f, 1f, 0f);
            case CameraPosition.EmptyDown: return new Rect(0f, 1f, 1f, 0f);
            default: return new Rect(0f, 0f, 0f, 0f);
        }
    }

    private CameraPosition ChangePosByConfiguration(CameraPosition cp)
    {
        switch (cp)
        {
            case CameraPosition.FullLeft: return CameraPosition.FullRight;
            case CameraPosition.FullRight: return CameraPosition.FullLeft;
            case CameraPosition.FullUp: return CameraPosition.FullDown;
            case CameraPosition.FullDown: return CameraPosition.FullUp;

            case CameraPosition.HalfLeft: return CameraPosition.HalfRight;
            case CameraPosition.HalfRight: return CameraPosition.HalfLeft;
            case CameraPosition.HalfUp: return CameraPosition.HalfDown;
            case CameraPosition.HalfDown: return CameraPosition.HalfUp;

            case CameraPosition.EmptyLeft: return CameraPosition.EmptyRight;
            case CameraPosition.EmptyRight: return CameraPosition.EmptyLeft;
            case CameraPosition.EmptyUp: return CameraPosition.EmptyDown;
            case CameraPosition.EmptyDown: return CameraPosition.EmptyUp;

            default: return cp;
        }
    }

    private CameraPosition ChangePosByOrientation(CameraPosition cp)
    {
        switch (cp)
        {
            case CameraPosition.FullLeft: return CameraPosition.FullUp;
            case CameraPosition.FullRight: return CameraPosition.FullDown;
            case CameraPosition.FullUp: return CameraPosition.FullLeft;
            case CameraPosition.FullDown: return CameraPosition.FullRight;

            case CameraPosition.HalfLeft: return CameraPosition.HalfUp;
            case CameraPosition.HalfRight: return CameraPosition.HalfDown;
            case CameraPosition.HalfUp: return CameraPosition.HalfLeft;
            case CameraPosition.HalfDown: return CameraPosition.HalfRight;

            case CameraPosition.EmptyLeft: return CameraPosition.EmptyUp;
            case CameraPosition.EmptyRight: return CameraPosition.EmptyDown;
            case CameraPosition.EmptyUp: return CameraPosition.EmptyLeft;
            case CameraPosition.EmptyDown: return CameraPosition.EmptyRight;

            default: return cp;
        }
    }

    private CameraPosition ChangePosByConnection(CameraPosition cp)
    {
        switch (cp)
        {
            case CameraPosition.FullLeft:
            case CameraPosition.EmptyLeft:
                return CameraPosition.HalfLeft;
            case CameraPosition.FullRight:
            case CameraPosition.EmptyRight:
                return CameraPosition.HalfRight;
            case CameraPosition.FullUp:
            case CameraPosition.EmptyUp:
                return CameraPosition.HalfUp;
            case CameraPosition.FullDown:
            case CameraPosition.EmptyDown:
                return CameraPosition.HalfDown;

            default: return cp;
        }
    }

    private CameraPosition ChangePosToFullByDisconnect(CameraPosition cp)
    {
        switch (cp)
        {
            case CameraPosition.HalfLeft: return CameraPosition.FullLeft;
            case CameraPosition.HalfRight: return CameraPosition.FullRight;
            case CameraPosition.HalfUp: return CameraPosition.FullUp;
            case CameraPosition.HalfDown: return CameraPosition.FullDown;
            default: return cp;
        }
    }

    private CameraPosition ChangePosToEmptyByDisconnect(CameraPosition cp)
    {
        switch (cp)
        {
            case CameraPosition.HalfLeft: return CameraPosition.EmptyLeft;
            case CameraPosition.HalfRight: return CameraPosition.EmptyRight;
            case CameraPosition.HalfUp: return CameraPosition.EmptyUp;
            case CameraPosition.HalfDown: return CameraPosition.EmptyDown;
            default: return cp;
        }
    }

    private CameraSize GetCameraSize(CameraPosition cp)
    {
        switch (cp)
        {
            case CameraPosition.FullLeft:
            case CameraPosition.FullRight:
            case CameraPosition.FullUp:
            case CameraPosition.FullDown:
                return CameraSize.Full;
            case CameraPosition.HalfLeft:
            case CameraPosition.HalfRight:
            case CameraPosition.HalfUp:
            case CameraPosition.HalfDown:
                return CameraSize.Half;
            case CameraPosition.EmptyLeft:
            case CameraPosition.EmptyRight:
            case CameraPosition.EmptyUp:
            case CameraPosition.EmptyDown:
                return CameraSize.None;
            default:
                return CameraSize.Full;
        }
    }

    private Rect LerpRect(Rect rect1, Rect rect2, float speed)
    {
        float x1 = rect1.x;
        float y1 = rect1.y;
        float w1 = rect1.width;
        float h1 = rect1.height;

        float x2 = rect2.x;
        float y2 = rect2.y;
        float w2 = rect2.width;
        float h2 = rect2.height;

        float x = Mathf.MoveTowards(x1, x2, speed * Time.deltaTime);
        float y = Mathf.MoveTowards(y1, y2, speed * Time.deltaTime);
        float w = Mathf.MoveTowards(w1, w2, speed * Time.deltaTime);
        float h = Mathf.MoveTowards(h1, h2, speed * Time.deltaTime);

        return new Rect(x, y, w, h);
    }

    private bool IsViewportEnabled(in Rect rect) => ViewportSize(rect) > deltaRounding;

    private bool IsViewportHalf(in Rect rect) => ViewportSize(rect) > (0.5f - deltaRounding);

    private float ViewportSize(in Rect rect) => rect.width * rect.height;

    private string PrintRect(in Rect rect) => rect.x.ToString() + " " + rect.y.ToString() + " " + rect.width.ToString() + " " + rect.height.ToString();

    private void DrawDividingLine()
    {
        dividingLine.startWidth = dividingLine.endWidth = orientation == Orientation.Vertical ? dividingLineWidth : dividingLineWidth * 2f;
        dividingLine.SetPosition(0, divPoint1);
        dividingLine.SetPosition(1, divPoint2);
    }

    private void SetDividingLine()
    {
        if (cameraPlayer1 && cameraPlayer2 && cameraPlayer1.enabled && cameraPlayer2.enabled)
        {
            dividingLine.enabled = cameraPlayer1.enabled && cameraPlayer2.enabled;
            if (orientation == Orientation.Vertical)
            {
                Camera rightCamera = posCamera1.ToString().Contains("Right") ? cameraPlayer1 : cameraPlayer2;
                divPoint1 = rightCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 1f));
                divPoint2 = rightCamera.ViewportToWorldPoint(new Vector3(0f, 1f, 1f));
                cameraPlayer1.fieldOfView = cameraPlayer2.fieldOfView = verticalFov;
            }
            else
            {
                Camera downCamera = posCamera1.ToString().Contains("Down") ? cameraPlayer1 : cameraPlayer2;
                divPoint1 = downCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 1f));
                divPoint2 = downCamera.ViewportToWorldPoint(new Vector3(1f, 0f, 1f));
                cameraPlayer1.fieldOfView = cameraPlayer2.fieldOfView = horizontalFov;
            }
        }
        else
        {
            dividingLine.enabled = false;
            if (cameraPlayer1) cameraPlayer1.fieldOfView = defaultFov;
            if (cameraPlayer2) cameraPlayer2.fieldOfView = defaultFov;
        }
    }

    public enum CameraSize
    {
        Full,
        Half,
        None
    }

    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public enum CamerasConfig
    {
        player1_player2,
        player2_player1
    }

    public enum CameraPosition
    {
        FullLeft,
        FullRight,
        FullUp,
        FullDown,
        HalfLeft,
        HalfRight,
        HalfUp,
        HalfDown,
        EmptyLeft,
        EmptyRight,
        EmptyUp,
        EmptyDown,
        None
    }

    public enum Players
    {
        Player1,
        Player2
    }
}