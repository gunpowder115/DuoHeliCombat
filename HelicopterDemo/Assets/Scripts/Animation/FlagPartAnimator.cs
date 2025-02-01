using UnityEngine;

public class FlagPartAnimator : MonoBehaviour
{
    //[SerializeField] private float angle = 30f;
    //[SerializeField] private float speed = 50f;
    [SerializeField] private float angleFrom = 10f;
    [SerializeField] private float angleTo = 30f;
    [SerializeField] private float speedFrom = 50f;
    [SerializeField] private float speedTo = 150f;

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
        //if (toRight)
        //{
        //    currAngle += speed * Time.deltaTime;
        //    if (currAngle >= angle)
        //    {
        //        currAngle = angle;
        //        toRight = false;
        //    }
        //}
        //else
        //{
        //    currAngle -= speed * Time.deltaTime;
        //    if (currAngle <= -angle)
        //    {
        //        currAngle = -angle;
        //        toRight = true;
        //    }
        //}
        //transform.localRotation = Quaternion.Euler(0f, currAngle, 0f);

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
        transform.localRotation = Quaternion.Euler(0f, currAngle, 0f);
    }
}
