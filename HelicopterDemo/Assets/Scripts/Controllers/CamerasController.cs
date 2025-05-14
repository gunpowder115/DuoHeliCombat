using UnityEngine;

public class CamerasController : MonoBehaviour
{
    [SerializeField] private bool isSingleCamera = false;
    [SerializeField] private float maxZoomOut = 3f;
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    private CameraMovement[] cameras;
    private Vector2 cameraInput1 => player1.InputDevice.GetCameraInput();
    private Vector2 cameraInput2 => player2.InputDevice.GetCameraInput();
    private Vector2 playerInput1 => player1.InputDevice.GetInput();
    private Vector2 playerInput2 => player2.InputDevice.GetInput();

    public static CamerasController Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;

        cameras = new CameraMovement[2];
        cameras[0] = player1.GetComponentInChildren<CameraMovement>();
        cameras[1] = player2.GetComponentInChildren<CameraMovement>();
    }

    private void Update()
    {
        if (isSingleCamera)
        {
            cameras[0].CameraInput = cameraInput1 + cameraInput2;
            cameras[0].PlayerInput = playerInput1 + playerInput2;
            cameras[1].CameraInput = cameraInput1 + cameraInput2;
            cameras[1].PlayerInput = playerInput1 + playerInput2;
        }
        else
        {
            cameras[0].CameraInput = cameraInput1;
            cameras[0].PlayerInput = playerInput1;
            cameras[1].CameraInput = cameraInput2;
            cameras[1].PlayerInput = playerInput2;
        }

        cameras[0].ContainerShift = isSingleCamera ? (player2.transform.position - player1.transform.position) / 2f : Vector3.zero;
        cameras[1].ContainerShift = -cameras[0].ContainerShift;
    }

    public void SetCamerasZoomOut(float currDist, float maxDist)
    {
        cameras[0].ZoomOut = cameras[1].ZoomOut = -maxZoomOut / maxDist * currDist;
    }
}
