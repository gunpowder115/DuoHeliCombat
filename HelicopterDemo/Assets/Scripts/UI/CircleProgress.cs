using UnityEngine;
using UnityEngine.UI;

public class CircleProgress : MonoBehaviour
{
    [SerializeField] private Image progressImage;
    [SerializeField] private Color fillColor = Color.gray;
    [SerializeField] private Color emptyColor = Color.red;

    private void Start()
    {
        SetFillColor();
    }

    public void SetCircleAmount(float amount)
    {
        progressImage.fillAmount = amount;
    }

    public void SetFillColor() => progressImage.color = fillColor;
    public void SetEmptyColor() => progressImage.color = emptyColor;
}