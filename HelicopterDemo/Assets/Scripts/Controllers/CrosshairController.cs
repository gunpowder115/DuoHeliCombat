using UnityEngine;
using static ViewPortController;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private float aimSpeed = 5f;
    [SerializeField] private float rayRadius = 1f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float targetHoldTime = 1f;
    [SerializeField] private GameObject aimItem1;
    [SerializeField] private GameObject targetAimItem1;
    [SerializeField] private GameObject aimItem2;
    [SerializeField] private GameObject targetAimItem2;

    private Camera camera1, camera2;
    private Crosshair crosshair1, crosshair2;

    public static CrosshairController singleton { get; private set; }

    private void Awake()
    {
        singleton = this;
    }

    public void SetCamera(Camera cam, Players playerNumber)
    {
        switch(playerNumber)
        {
            case Players.Player1:
                camera1 = cam;
                crosshair1 = new Crosshair(camera1, aimItem1, targetAimItem1, targetHoldTime, aimSpeed, rayRadius, maxDistance);
                break;
            case Players.Player2:
                camera2 = cam;
                crosshair2 = new Crosshair(camera2, aimItem2, targetAimItem2, targetHoldTime, aimSpeed, rayRadius, maxDistance);
                break;
        }
    }

    public Crosshair GetCrosshair(Players playerNumber)
    {
        switch(playerNumber)
        {
            case Players.Player2:
                return crosshair2;
            default:
                return crosshair1;
        }
    }
}
