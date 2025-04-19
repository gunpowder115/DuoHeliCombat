using UnityEngine;
using UnityEngine.UI;

public class CircleProgress : MonoBehaviour
{
    [SerializeField] private bool useInvert = false;
    [SerializeField] private Image progressImage;
    [SerializeField] private Image invertProgressImage;
    [SerializeField] private Color fillColor = Color.gray;
    [SerializeField] private Color emptyColor = Color.red;

    private void Start()
    {
        SetFillColor();
        if (useInvert)
        {
            invertProgressImage.color = emptyColor;
            invertProgressImage.fillAmount = 0f;
        }
    }

    public void SetCircleAmount(float amount)
    {
        progressImage.fillAmount = amount;
        if(useInvert)
            invertProgressImage.fillAmount = 1 - amount;
    }

    public void SetFillColor() => progressImage.color = fillColor;
    public void SetEmptyColor() => progressImage.color = emptyColor;
}