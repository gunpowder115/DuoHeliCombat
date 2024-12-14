using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnimator : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private bool xOffset = true;
    [SerializeField] private bool yOffset = true;

    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Time.time * speed;
        rend.material.SetTextureOffset("_MainTex", new Vector2(xOffset ? offset : 0, yOffset ? offset : 0));
    }
}
