using UnityEngine;

public class RadialMenuLayout : MonoBehaviour
{
    [SerializeField] private GameObject[] icons;
    [SerializeField] private float radius = 150f;

    public GameObject[] Icons => icons;

    void Start()
    {
        ArrangeIcons();
    }

    public void ArrangeIcons()
    {
        float angleStep = 360f / icons.Length;
        for (int i = 0; i < icons.Length; i++)
        {
            float angle = (-i * angleStep + 90f) * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            if (icons[i])
                icons[i].GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
}