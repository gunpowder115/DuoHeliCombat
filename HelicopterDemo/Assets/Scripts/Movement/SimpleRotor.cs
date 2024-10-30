using UnityEngine;

public class SimpleRotor : MonoBehaviour
{
    [SerializeField] private bool alwaysRot = false;
    [SerializeField] private bool aroundSelf = true;
    [SerializeField] private float rotSpeed = 5.0f;
    [SerializeField] private float rotAcceleration = 0.2f;
    [SerializeField] private float spdCoefToTakeoff = 0.9f;
    [SerializeField] private Axes axis = Axes.X;

    private float currRotorSpeed, tgtRotorSpeed;

    public bool ReadyToTakeoff => currRotorSpeed >= rotSpeed * spdCoefToTakeoff;

    // Update is called once per frame
    void Update()
    {
        if (alwaysRot)
        {
            Vector3 rotation = new Vector3(axis == Axes.X ? rotSpeed : 0f,
                                            axis == Axes.Y ? rotSpeed : 0f,
                                            axis == Axes.Z ? rotSpeed : 0f);

            transform.Rotate(rotation, aroundSelf ? Space.Self : Space.World);
        }
        else
        {
            currRotorSpeed = Mathf.Lerp(currRotorSpeed, tgtRotorSpeed, rotAcceleration * Time.deltaTime);
            Vector3 rotation = new Vector3(axis == Axes.X ? currRotorSpeed : 0f,
                                            axis == Axes.Y ? currRotorSpeed : 0f,
                                            axis == Axes.Z ? currRotorSpeed : 0f);
            transform.Rotate(rotation);
        }
    }

    public void StartRotor() => tgtRotorSpeed = rotSpeed;
    public void FastStartRotor() => tgtRotorSpeed = currRotorSpeed = rotSpeed;
    public void StopRotor() => tgtRotorSpeed = 0f;

    public enum Axes
    {
        X, Y, Z
    }
}
