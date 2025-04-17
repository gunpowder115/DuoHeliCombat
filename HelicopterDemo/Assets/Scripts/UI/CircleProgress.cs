using UnityEngine;
using UnityEngine.UI;

public class CircleProgress : MonoBehaviour
{
    [SerializeField] private Image roundImage;
    [SerializeField] private Color fillColor = Color.gray;
    [SerializeField] private Color emptyColor = Color.red;

    private void Start()
    {
        SetFillColor();
    }

    public void SetCircleAmount(float amount)
    {
        roundImage.fillAmount = amount;
    }

    public void SetFillColor() => roundImage.color = fillColor;
    public void SetEmptyColor() => roundImage.color = emptyColor;
}