using System.Collections.Generic;
using UnityEngine;

public class CamerasController : MonoBehaviour
{
    [SerializeField] private bool isSingleCamera = false;
    [SerializeField] private float maxZoomOut = 3f;
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    private List<CameraMovement> cameras;
    private Vector2 cameraInput1 => player1.InputDevice.GetCameraInput();
    private Vector2 cameraInput2 => player2.InputDevice.GetCameraInput();
    private Vector2 playerInput1 => player1.InputDevice.GetInput();
    private Vector2 playerInput2 => player2.InputDevice.GetInput();

    public static CamerasController Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;

        cameras = new List<CameraMovement>();
        if (player1) cameras.Add(player1.GetComponentInChildren<CameraMovement>());
        if (player2) cameras.Add(player2.GetComponentInChildren<CameraMovement>());
    }

    private void Update()
    {
        if (isSingleCamera)
        {
            foreach (var cam in cameras)
            {
                cam.CameraInput = cameraInput1 + cameraInput2;
                cam.PlayerInput = playerInput1 + playerInput2;
            }
        }
        else
        {
            cameras[0].CameraInput = cameraInput1;
            cameras[0].PlayerInput = playerInput1;
            if (cameras.Count > 1)
            {
                cameras[1].CameraInput = cameraInput2;
                cameras[1].PlayerInput = playerInput2;
            }
        }

        cameras[0].ContainerShift = isSingleCamera ? (player2.transform.position - player1.transform.position) / 2f : Vector3.zero;
        if (cameras.Count > 1)
            cameras[1].ContainerShift = -cameras[0].ContainerShift;
    }

    public void SetCamerasZoomOut(float currDist, float maxDist)
    {
        cameras[0].ZoomOut = cameras[1].ZoomOut = -maxZoomOut / maxDist * currDist;
    }
}
