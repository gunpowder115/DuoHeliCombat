using UnityEngine;
using static Types;

public class FlagPartAnimator : MonoBehaviour
{
    [SerializeField] private float angleFrom = 10f;
    [SerializeField] private float angleTo = 30f;
    [SerializeField] private float speedFrom = 50f;
    [SerializeField] private float speedTo = 150f;
    [SerializeField] private Axes axis = Axes.Y;

    private bool toRight;
    private float currAngle;
    private float angle, speed;

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
                angle = Random.Range(angleFrom, angleTo);
                speed = Random.Range(speedFrom, speedTo);
                toRight = false;
            }
        }
        else
        {
            currAngle -= speed * Time.deltaTime;
            if (currAngle <= -angle)
            {
                currAngle = -angle;
                angle = Random.Range(angleFrom, angleTo);
                speed = Random.Range(speedFrom, speedTo);
                toRight = true;
            }
        }

        float x = 0f, y = 0f, z = 0f;
        switch(axis)
        {
            case Axes.X: x = currAngle; break;
            case Axes.Y: y = currAngle; break;
            case Axes.Z: z = currAngle; break;
        }

        transform.localRotation = Quaternion.Euler(x, y, z);
    }
}
