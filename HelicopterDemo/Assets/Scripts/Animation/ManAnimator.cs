using UnityEngine;

public class ManAnimator : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float walkingCoef = 0.1f;
    [SerializeField] private float limbMaxAngle = 30f;
    [SerializeField] private GameObject[] limbs;
    [SerializeField] private GameObject head;

    private bool toRight, isWalking;
    private float currAngle;
    private Vector3 prisonCenter;
    private GameObject helicopter;

    private void Update()
    {
        if (isWalking)
        {
            MoveLimbs();
            transform.Translate(transform.forward * speed * walkingCoef * Time.deltaTime);
            head.transform.LookAt(helicopter.transform);
            if (Vector3.Distance(prisonCenter, transform.position) < 0.5f)
                isWalking = false;
        }
    }

    public void SetBorders(Vector3 center, float xMin, float xMax, float zMin, float zMax)
    {
        prisonCenter = center;

        float coef = 0.85f;
        float x = (xMin + xMax) * coef;
        float z = (zMin + zMax) * coef;
        transform.position = center + new Vector3(x, -center.y / 0.93f, z);
        transform.LookAt(new Vector3(center.x, transform.position.y, center.z));
    }

    public void SetArrivingHelicopter(GameObject helicopter)
    {
        this.helicopter = helicopter;
        isWalking = true;
    }

    private void MoveLimbs()
    {
        if (toRight)
        {
            currAngle += speed * Time.deltaTime;
            if (currAngle >= limbMaxAngle)
            {
                currAngle = limbMaxAngle;
                toRight = false;
            }
        }
        else
        {
            currAngle -= speed * Time.deltaTime;
            if (currAngle <= -limbMaxAngle)
            {
                currAngle = -limbMaxAngle;
                toRight = true;
            }
        }

        if (limbs != null && limbs.Length > 0)
            for (int i = 0; i < limbs.Length; i++)
                limbs[i].transform.localRotation = Quaternion.Euler(i % 2 == 0 ? currAngle : -currAngle, 0f, 0f);
    }
}