using UnityEngine;

public class CargoMover : MonoBehaviour
{
    [SerializeField] private float speed = 3f;

    private Vector3 translation;

    private void Start()
    {
        translation = new Vector3(0f, -speed * Time.deltaTime, 0f);
    }

    private void Update()
    {
        transform.Translate(translation);
    }
}
