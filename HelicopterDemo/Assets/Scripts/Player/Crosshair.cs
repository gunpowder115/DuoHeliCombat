using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Vector2 ToTargetSelection => toTargetSelection;
    public Vector3 HitPoint { get; private set; }
    public GameObject SelectedTarget { get; private set; }

    private GameObject aimItem;
    private GameObject targetAimItem;
    private Camera crosshairCamera;
    private float currHoldTime, targetHoldTime;
    private float aimSpeed;
    private float rayRadius;
    private float maxDistance;
    private Vector2 toTargetSelection, targetScreenPos;

    public Crosshair(Camera camera, GameObject aimItem, GameObject tgtAimItem, float tgtHoldTime, float aimSpeed, float rayRadius, float maxDist)
    {
        this.crosshairCamera = camera;
        this.aimItem = aimItem;
        this.targetAimItem = tgtAimItem;
        this.targetHoldTime = tgtHoldTime;
        this.aimSpeed = aimSpeed;
        this.rayRadius = rayRadius;
        this.maxDistance = maxDist;
    }

    void Start()
    {
        aimItem.SetActive(false);
        targetAimItem.SetActive(false);
    }

    public void InitCrosshair()
    {

    }

    public void Show()
    {
        aimItem.SetActive(true);
        aimItem.transform.position = new Vector3(crosshairCamera.pixelWidth / 2f, crosshairCamera.pixelHeight / 2f, 0f);
    }

    public void Hide()
    {
        aimItem.SetActive(false);
        targetAimItem.SetActive(false);
        SelectedTarget = null;
        currHoldTime = 0f;
    }

    public void Translate(Vector2 direction)
    {
        float aimX, aimY;
        aimX = Limit(aimItem.transform.position.x + direction.x * aimSpeed, 0f, crosshairCamera.pixelWidth);
        aimY = Limit(aimItem.transform.position.y + direction.y * aimSpeed, 0f, crosshairCamera.pixelHeight);
        aimItem.transform.position = new Vector3(aimX, aimY, 0f);

        float cameraCoefX = 2f * aimX / crosshairCamera.pixelWidth - 1f;
        float cameraCoefY = 2f * aimY / crosshairCamera.pixelHeight - 1f;
        toTargetSelection = new Vector2(cameraCoefX, -cameraCoefY);

        SetTargetAim();
    }

    private void SetTargetAim()
    {
        Ray ray = crosshairCamera.ScreenPointToRay(aimItem.transform.position);
        var raycastHits = Physics.SphereCastAll(ray, rayRadius, maxDistance);
        bool hitEnemy = false;
        for (int i = 0; i < raycastHits.Length; i++)
        {
            if (i == 0) HitPoint = raycastHits[i].point;
            var hitObject = raycastHits[i].transform.gameObject;
            if (hitObject.GetComponent<Npc>())
            {
                targetScreenPos = crosshairCamera.WorldToScreenPoint(hitObject.transform.position);
                Ray rayCenter = crosshairCamera.ScreenPointToRay(targetScreenPos);
                Physics.Raycast(rayCenter, out RaycastHit hitCenter);
                var hitCenterObject = hitCenter.transform.gameObject;
                if (hitObject == hitCenterObject)
                {
                    currHoldTime = targetHoldTime;
                    SelectedTarget = hitObject;
                    HitPoint = hitObject.transform.position;
                    hitEnemy = true;
                    break;
                }
            }
        }

        if (!hitEnemy)
        {
            if ((currHoldTime -= Time.deltaTime) > 0f && SelectedTarget)
            {
                HitPoint = SelectedTarget.transform.position;
                targetScreenPos = crosshairCamera.WorldToScreenPoint(SelectedTarget.transform.position);
                targetAimItem.transform.position = new Vector3(targetScreenPos.x, targetScreenPos.y, targetAimItem.transform.position.z);
                targetAimItem.SetActive(true);
            }
            else
            {
                SelectedTarget = null;
                targetAimItem.SetActive(false);
            }
        }
        else
        {
            targetAimItem.transform.position = new Vector3(targetScreenPos.x, targetScreenPos.y, targetAimItem.transform.position.z);
            targetAimItem.SetActive(true);
        }
    }

    private float Limit(float value, float min, float max)
    {
        if (value <= min) return min;
        else if (value >= max) return max;
        else return value;
    }
}
