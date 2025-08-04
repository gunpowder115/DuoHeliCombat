using UnityEngine;

public class WaterAnimator : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private bool xOffset = true;
    [SerializeField] private bool yOffset = true;

    private Renderer rend;
    private Material material;

    void Start()
    {
        rend = GetComponent<Renderer>();
        material = rend.material;
        material.SetFloat("_Speed", speed);
        material.SetFloat("_xOffset", xOffset ? 1f : 0f);
        material.SetFloat("_yOffset", yOffset ? 1f : 0f);
    }
}
