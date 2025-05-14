using UnityEngine;

public class RigidbodyTranslation : Translation
{
    private new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rigidbody)
        {
            if (IsTowing)
                movement = TowingMovement;
            else
            {
                movement = new Vector3(speed.x, 0f, speed.z);
                movement = Vector3.ClampMagnitude(movement, speedAbs);
                movement = new Vector3(movement.x, speed.y, movement.z);
            }

            rigidbody.velocity = movement * Time.fixedDeltaTime;
        }
    }
}
