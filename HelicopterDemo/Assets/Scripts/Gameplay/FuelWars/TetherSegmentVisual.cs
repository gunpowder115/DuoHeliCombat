using UnityEngine;

public class TetherSegmentVisual : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [SerializeField] private float baseLength = 0.5f;

    public Transform Target { get; set; }
    public bool IsVisible { get; set; }
    public bool CollidesWithFuelTower { get; private set; }

    void LateUpdate()
    {
        if (Target == null) return;

        Vector3 dir = Target.position - transform.position;
        float currentLength = dir.magnitude;

        visual.position = transform.position + dir * 0.5f;

        if (dir.sqrMagnitude > 0.0001f)
            visual.rotation = Quaternion.LookRotation(dir);

        float scaleFactor = currentLength / baseLength;
        visual.localScale = new Vector3(1, 1, scaleFactor);

        if (!IsVisible)
            visual.gameObject.SetActive(IsVisible);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollidesWithFuelTower = collision.gameObject.GetComponent<FuelTower>();
    }
}