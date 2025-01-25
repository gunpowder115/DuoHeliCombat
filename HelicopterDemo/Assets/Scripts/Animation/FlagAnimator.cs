using UnityEngine;

public class FlagAnimator : MonoBehaviour
{
    [SerializeField] private float angle = 30f;
    [SerializeField] private float speed = 50f;

    private bool toRight;
    private float currAngle;

    private void Start()
    {
        toRight = true;
        currAngle = 0f;
    }

    void Update()
    {
        if (toRight)
        {
            currAngle += speed * Time.deltaTime;
            if (currAngle >= angle)
            {
                currAngle = angle;
                toRight = false;
            }
        }
        else
        {
            currAngle -= speed * Time.deltaTime;
            if (currAngle <= -angle)
            {
                currAngle = -angle;
                toRight = true;
            }
        }
        transform.rotation = Quaternion.Euler(0f, currAngle, 0f);
    }
}
