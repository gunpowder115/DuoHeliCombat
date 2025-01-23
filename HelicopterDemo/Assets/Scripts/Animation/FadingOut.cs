using UnityEngine;

public class FadingOut : MonoBehaviour
{
    [SerializeField] private float fadeOutSpeed = 0.1f;
    [SerializeField] private float waitTime = 1f;

    private Renderer[] rends;
    private float currTime;

    void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        currTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;

        if (currTime > waitTime)
        {
            foreach (var rend in rends)
            {
                Color color = rend.material.color;
                color.a -= fadeOutSpeed * Time.deltaTime;
                rend.material.color = color;

                if (color.a <= 0f)
                    Destroy(gameObject);
            }
        }
    }
}
