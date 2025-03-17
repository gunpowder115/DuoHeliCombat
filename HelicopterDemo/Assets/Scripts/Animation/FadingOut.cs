using UnityEngine;

public class FadingOut : MonoBehaviour
{
    [SerializeField] private bool destroy = true;
    [SerializeField] private bool defaultVisible = true;
    [SerializeField] private bool active = true;
    [SerializeField] private float fadeOutSpeed = 0.5f;
    [SerializeField] private float waitTime = 10f;
    [SerializeField] private float defaultAlpha = 1f;

    private Renderer[] rends;
    private float currAlpha;

    public bool Active
    {
        get => active;
        set => active = value;
    }
    public float CurrTime { get; set; }

    void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        CurrTime = defaultVisible ? 0f : Mathf.Infinity;
        currAlpha = defaultAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            CurrTime += Time.deltaTime;

            if (CurrTime > waitTime)
            {
                currAlpha -= fadeOutSpeed * Time.deltaTime;
                if (currAlpha < 0f) currAlpha = 0f;
            }
            else
                currAlpha = defaultAlpha;

            foreach (var rend in rends)
            {
                Color color = rend.material.color;
                color.a = currAlpha;
                rend.material.color = color;

                if (color.a <= 0f && destroy)
                    Destroy(gameObject);
            }
        }
    }
}
