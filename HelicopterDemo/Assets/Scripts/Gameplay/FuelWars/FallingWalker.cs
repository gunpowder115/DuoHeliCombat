using UnityEngine;

public class FallingWalker : MonoBehaviour
{
    [SerializeField] private GameObject walkerModel;
    [SerializeField] private GameObject rotatingPart;
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;
    [SerializeField] private GameObject brokenLegPrefab;
    [SerializeField] private GameObject littleDustEffectPrefab;
    [SerializeField] private GameObject hugeDustEffectPrefab;
    [SerializeField] private GameObject fallenWalkerPrefab;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float fallMaxAngle = 60f;

    private float xPos;
    private FallingState fallingState;
    private GameObject destroyingLeg;
    private Leg leg;
    private Quaternion rotatingPartEnd;
    private bool rotPartToForward;

    private void Awake()
    {
        fallingState = FallingState.None;
    }

    private void Update()
    {
        switch (fallingState)
        {
            case FallingState.LegDestroying:
                if (brokenLegPrefab)
                    Instantiate(brokenLegPrefab, destroyingLeg.transform.position, destroyingLeg.transform.rotation);
                if (littleDustEffectPrefab)
                    Instantiate(littleDustEffectPrefab, transform.position + (leg == Leg.Left ? -transform.right : transform.right) * 6f, transform.rotation);
                Destroy(destroyingLeg);
                fallingState = FallingState.Falling;
                break;
            case FallingState.Falling:
                if (Falling())
                {
                    Vector3 right = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
                    if (hugeDustEffectPrefab)
                        Instantiate(hugeDustEffectPrefab, transform.position + (leg == Leg.Left ? -right : right) * 8f, Quaternion.identity);

                    if (fallenWalkerPrefab)
                    {
                        var fallen = Instantiate(fallenWalkerPrefab, transform.position, Quaternion.identity).GetComponent<FallenWalker>();
                        fallen.SetFallenParams(leg == Leg.Left, 
                            (leg == Leg.Right && rotPartToForward) || (leg == Leg.Left && !rotPartToForward), 
                            rotatingPart.transform.localRotation, transform.rotation);
                    }

                    Destroy(gameObject);
                    fallingState = FallingState.None;
                }
                break;
        }
    }

    public void SetFallingParams(bool destroyingLeft, float rotAngle)
    {
        fallingState = FallingState.LegDestroying;
        leg = destroyingLeft ? Leg.Left : Leg.Right;
        destroyingLeg = destroyingLeft ? leftLeg : rightLeg;

        xPos = Mathf.Abs(leftLeg.transform.localPosition.x);
        walkerModel.transform.localPosition += new Vector3(destroyingLeft ? -xPos : xPos, 0f, 0f);
        rotatingPart.transform.localRotation = Quaternion.Euler(0f, rotAngle, 0f);

        if (Vector3.Dot(rotatingPart.transform.forward, transform.forward) >= 0f)
        {
            rotatingPartEnd = Quaternion.Euler(transform.forward);
            rotPartToForward = true;
        }
        else
        {
            rotatingPartEnd = Quaternion.Euler(-transform.forward);
            rotPartToForward = false;
        }
    }

    private bool Falling()
    {
        speed += acceleration * Time.deltaTime;
        transform.Rotate(0f, 0f, (leg == Leg.Left ? speed : -speed) * Time.deltaTime);
        float currAngle = transform.localRotation.eulerAngles.z;
        if (currAngle > 180f) currAngle -= 360f;

        rotatingPart.transform.localRotation = Quaternion.RotateTowards(rotatingPart.transform.localRotation, rotatingPartEnd, speed * Time.deltaTime);

        return (leg == Leg.Left && currAngle >= fallMaxAngle) || (leg == Leg.Right && currAngle <= -fallMaxAngle);
    }

    private enum FallingState
    {
        LegDestroying,
        Falling,
        None
    }

    private enum Leg
    {
        Left,
        Right
    }
}
