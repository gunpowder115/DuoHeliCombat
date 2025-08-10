using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class FallingHelicopter : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 300f;
    [SerializeField] private float fallingSpeed = 100f;

    public GameObject DeadPrefab { get; set; }
    public GameObject ExplosionPrefab { get; set; }

    private bool isCrashed;
    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isCrashed)
        {
            if (DeadPrefab)
                Instantiate(DeadPrefab, transform.position, transform.rotation);

            if (ExplosionPrefab)
                Instantiate(ExplosionPrefab, gameObject.transform.position, gameObject.transform.rotation);

            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        rigidbody.AddRelativeTorque(0f, rotationSpeed * Time.fixedDeltaTime, 0f);
        rigidbody.AddRelativeForce(fallingSpeed * Time.fixedDeltaTime, 0f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.GetComponent<Projectile>())
            isCrashed = true;
    }
}
