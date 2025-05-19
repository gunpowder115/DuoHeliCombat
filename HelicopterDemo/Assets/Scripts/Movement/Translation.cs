using UnityEngine;

public class Translation : MonoBehaviour
{
    [SerializeField] private float maxHeight = 50.0f;
    [SerializeField] private float minHeight = 10.0f;

    public Vector3 TargetDirectionNorm => new Vector3(speed.x, 0f, speed.z).normalized;
    public bool IsHeightBorder => this.gameObject.transform.position.y >= maxHeight || this.gameObject.transform.position.y <= minHeight;
    public bool RotToDir { get; private set; }
    public CameraMovement CameraMovement { get; set; }
    public Vector3 Movement => movement;
    public bool IsTowing { get; set; }
    public float TowingCoef { get; set; }

    protected Vector3 speed, movement;
    protected float speedAbs, verticalSpeedAbs;

    public void SetHorizontalTranslation(Vector3 speed)
    {
        if (CameraMovement)
            speed = Quaternion.Euler(0f, CameraMovement.ContainerRotation.y, 0f) * speed;

        speedAbs = speed.magnitude;
        this.speed = new Vector3(speed.x, this.speed.y, speed.z);
    }

    public void SetRelToTargetTranslation(Vector3 speed, float angle)
    {
        speedAbs = speed.magnitude;
        Vector3 temp = Quaternion.Euler(0f, angle, 0f) * speed;
        this.speed = new Vector3(temp.x, this.speed.y, temp.z);
    }

    public void SetVerticalTranslation(float speed)
    {
        this.speed = new Vector3(this.speed.x, speed, this.speed.z);
    }

    public void SetGlobalTranslation(Vector3 speed)
    {
        speedAbs = new Vector3(speed.x, 0f, speed.z).magnitude;
        this.speed = new Vector3(speed.x, speed.y, speed.z);
    }

    public bool SwitchRotation()
    {
        RotToDir = !RotToDir;
        return RotToDir;
    }
}