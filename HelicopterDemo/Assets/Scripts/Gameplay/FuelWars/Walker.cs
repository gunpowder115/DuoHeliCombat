using UnityEngine;

public class Walker : MonoBehaviour
{
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float legSpeedCoef = 1.2f;
    [SerializeField] private float legTilt = 10f;
    [SerializeField] private float pause = 0.5f;

    private bool isStep, isPause;
    private float zPos, xPos;
    private float legSpeed;
    private float currLegAngle, currLegRadius;
    private float pauseTime;
    private LegState leftLegState, rightLegState;
    private Vector3 legCenter;

    private void Start()
    {
        zPos = Mathf.Abs(leftLeg.transform.localPosition.z);
        xPos = Mathf.Abs(leftLeg.transform.localPosition.x);

        if (leftLeg.transform.localPosition.z == rightLeg.transform.localPosition.z)
            leftLegState = rightLegState = LegState.Middle;
        else if (leftLeg.transform.localPosition.z < rightLeg.transform.localPosition.z)
        {
            leftLegState = LegState.Back;
            rightLegState = LegState.Front;
        }
        else
        {
            leftLegState = LegState.Front;
            rightLegState = LegState.Back;
        }

        legSpeed = legSpeedCoef * speed;
        leftLeg.transform.parent = rightLeg.transform.parent = null;
        legCenter = new Vector3(xPos, leftLeg.transform.localPosition.y, 0f);
        currLegRadius = zPos;
    }

    private void Update()
    {
        GameObject currLeg = leftLegState == LegState.Back ? leftLeg : rightLeg;

        if (isPause)
        {
            pauseTime += Time.deltaTime;
            if (pauseTime >= pause)
            {
                pauseTime = 0f;
                isPause = false;
            }
        }
        else if (Step(currLeg))
        {
            leftLegState = leftLegState == LegState.Back ? LegState.Front : LegState.Back;
            rightLegState = leftLegState == LegState.Back ? LegState.Front : LegState.Back;
            isStep = false;
            isPause = true;
        }

        Debug.Log(currLegAngle);
    }

    private bool Step(GameObject leg)
    {
        if (!isStep)
        {
            isStep = true;
            currLegAngle = 0f;
            leg.transform.parent = transform;
        }

        if (currLegAngle >= Mathf.PI)
        {
            leg.transform.parent = null;
            return true;
        }
        else
        {
            currLegAngle += legSpeed * Time.deltaTime;
            float z = legCenter.z - Mathf.Cos(-currLegAngle) * currLegRadius;
            float y = legCenter.y - Mathf.Sin(-currLegAngle) * currLegRadius;
            leg.transform.localPosition = new Vector3(leg.transform.localPosition.x, y, z);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            leg.transform.localRotation = Quaternion.Euler(GetLegTilt(currLegAngle), 0f, 0f);

            return false;
        }
    }

    private float GetLegTilt(float radAngle)
    {
        if (radAngle >= 0f && radAngle < Mathf.PI / 4f)
            return 4f * legTilt / Mathf.PI * radAngle;
        else if (radAngle >= Mathf.PI / 4f && radAngle < 3f * Mathf.PI / 4f)
            return -4f * legTilt / Mathf.PI * radAngle + 2f * legTilt;
        else
            return 4f * legTilt / Mathf.PI * radAngle - 4f * legTilt;
    }

    public enum LegState
    {
        Front,
        Middle,
        Back
    }
}
