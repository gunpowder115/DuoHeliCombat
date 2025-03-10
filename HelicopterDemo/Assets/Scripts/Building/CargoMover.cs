using UnityEngine;

public class CargoMover : MonoBehaviour
{
    [SerializeField] private float openingTime = 0.5f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float fastSpeedCoef = 1.5f;

    private float currTime;
    private Vector3 translation;

    private void Start()
    {
        translation = new Vector3(0f, -speed * fastSpeedCoef * Time.deltaTime, 0f);
    }

    private void Update()
    {
        if (currTime > openingTime)
            translation = new Vector3(0f, -speed * Time.deltaTime, 0f);
        else
            currTime += Time.deltaTime;
        transform.Translate(translation);
    }
}
