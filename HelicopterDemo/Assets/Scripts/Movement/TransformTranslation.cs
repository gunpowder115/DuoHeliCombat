using UnityEngine;

public class TransformTranslation : Translation
{
    private void Update()
    {
        movement = new Vector3(speed.x, 0f, speed.z);
        movement = Vector3.ClampMagnitude(movement, speedAbs);
        movement = new Vector3(movement.x, speed.y, movement.z);

        movement = transform.InverseTransformDirection(movement);
        transform.Translate(movement * Time.deltaTime);
    }
}
