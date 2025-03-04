using UnityEngine;

public class BombingCamera : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float height = 20f;
    [SerializeField] private BombingCameraType cameraType = BombingCameraType.firstView;
    [SerializeField] private Camera firstViewCamera;
    [SerializeField] private Camera sideViewCamera;

    private GameObject cameraViewPoint;

    private void Start()
    {
        cameraViewPoint = new GameObject();
        Vector3 cameraViewPos = transform.position;
        cameraViewPos.y = height;
        cameraViewPoint.transform.position = cameraViewPos;

        firstViewCamera.enabled = cameraType == BombingCameraType.firstView;
        sideViewCamera.enabled = cameraType == BombingCameraType.sideView;
    }

    private void Update()
    {
        if (cameraType == BombingCameraType.sideView)
        {
            Vector3 cameraViewPos = transform.position;
            cameraViewPos.y = height;
            cameraViewPoint.transform.position = cameraViewPos;

            Vector3 moving = new Vector3(speed * Time.deltaTime, 0f, 0f);
            sideViewCamera.transform.Translate(moving);
            sideViewCamera.transform.LookAt(cameraViewPoint.transform);
        }
    }

    private enum BombingCameraType
    {
        firstView,
        sideView
    }
}