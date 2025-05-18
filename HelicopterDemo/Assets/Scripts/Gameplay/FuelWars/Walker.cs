using Assets.Scripts.Gameplay.FuelWars;
using UnityEngine;

public class Walker : MonoBehaviour, IDestroyableByTether
{
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float stepDelta = 1.3f;
    [SerializeField] private float legSpeedCoef = 1.2f;
    [SerializeField] private float legTilt = 10f;
    [SerializeField] private float pause = 0.5f;
    [SerializeField] private float kneeTilt = -20f;
    [SerializeField] private GameObject destroyEffectPrefab;
    [SerializeField] private GameObject destroyedPrefab;

    private bool isStep, isPause, isWalking;
    private bool isFirstStep, isLastStep;
    private bool stopRequest;
    private float yPos;
    private float legSpeed;
    private float currLegAngle, currLegRadius;
    private float pauseTime;
    private LegState leftLegState, rightLegState;
    private Vector3 legCenter;
    private GameObject currLeg, currKnee;
    private DestroyableByTetherController destroyableByTetherController;

    private void Awake()
    {
        destroyableByTetherController = DestroyableByTetherController.Singleton;
        destroyableByTetherController.AddItem(this);
    }

    private void Start()
    {
        yPos = Mathf.Abs(leftLeg.transform.localPosition.y);

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

        currLeg = rightLeg;
        currKnee = currLeg.GetComponentInChildren<WalkerKnee>().gameObject;

        isFirstStep = true;
        isWalking = true;
    }

    private void Update()
    {
        if (isPause)
        {
            pauseTime += Time.deltaTime;
            if (pauseTime >= pause)
            {
                pauseTime = 0f;
                isPause = false;
            }
        }
        else if (isWalking)
        {
            if (isFirstStep)
            {
                if (Step(currLeg, StepType.First))
                {
                    leftLegState = LegState.Back;
                    rightLegState = LegState.Front;
                    isStep = false;
                    isPause = true;
                    isFirstStep = false;

                    currLeg = currLeg == rightLeg ? leftLeg : rightLeg;
                    currKnee = currLeg.GetComponentInChildren<WalkerKnee>().gameObject;
                }
            }
            else if (isLastStep)
            {
                if (Step(currLeg, StepType.Last))
                {
                    leftLegState = rightLegState = LegState.Middle;
                    isStep = false;
                    isPause = true;
                    isLastStep = false;
                    isWalking = false;

                    currLeg = currLeg == rightLeg ? leftLeg : rightLeg;
                    currKnee = currLeg.GetComponentInChildren<WalkerKnee>().gameObject;
                }
            }
            else if (Step(currLeg))
            {
                leftLegState = leftLegState == LegState.Back ? LegState.Front : LegState.Back;
                rightLegState = leftLegState == LegState.Back ? LegState.Front : LegState.Back;
                isStep = false;
                isPause = true;

                currLeg = leftLegState == LegState.Back ? leftLeg : rightLeg;
                currKnee = currLeg.GetComponentInChildren<WalkerKnee>().gameObject;

                if (stopRequest) isLastStep = true;
            }
        }
    }

    public void StartWalker()
    {
        isWalking = true;
        isFirstStep = true;
    }

    public void StopWalker() => stopRequest = true;

    public void CallToDestroy()
    {
        if (destroyEffectPrefab) Instantiate(destroyEffectPrefab);
        Destroy(gameObject);
        Destroy(leftLeg);
        Destroy(rightLeg);
        if (destroyedPrefab) Instantiate(destroyedPrefab);
    }

    private bool Step(GameObject leg, StepType stepType = StepType.Usual)
    {
        float currLinearSpeed;
        if (stepType == StepType.Usual)
        {
            legCenter = new Vector3(leg.transform.localPosition.x, yPos, 0f);
            currLegRadius = stepDelta;
            currLinearSpeed = speed;
        }
        else
        {
            legCenter = new Vector3(leg.transform.localPosition.x, yPos, (stepType == StepType.First ? stepDelta / 2f : -stepDelta / 2f));
            currLegRadius = stepDelta / 2f;
            currLinearSpeed = speed / 2f;
        }

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
            transform.Translate(Vector3.forward * currLinearSpeed * Time.deltaTime);

            leg.transform.localRotation = Quaternion.Euler(GetLegTilt(currLegAngle), 0f, 0f);
            currKnee.transform.localRotation = Quaternion.Euler(GetKneeTilt(currLegAngle), 0f, 0f);

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

    private float GetKneeTilt(float radAngle)
    {
        if (radAngle >= 0f && radAngle < Mathf.PI / 2f)
            return 2f * kneeTilt / Mathf.PI * radAngle;
        else
            return -2f * kneeTilt / Mathf.PI * radAngle + 2f * kneeTilt;
    }

    public enum LegState
    {
        Front,
        Middle,
        Back
    }

    public enum StepType
    {
        First,
        Usual,
        Last
    }
}
