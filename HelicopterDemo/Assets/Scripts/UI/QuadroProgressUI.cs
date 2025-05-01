using UnityEngine;
using UnityEngine.UI;

public class QuadroProgressUI : MonoBehaviour
{
    [SerializeField] private Image[] progressImages;
    [SerializeField] private Image iconImage;
    [SerializeField] private Color emptyColor = Color.red;

    public Color FillColor { get; set; }

    private void Start()
    {
        SetAllAsFilled();
    }

    public void SetCircleAmount(float amount)
    {
        SetAllAsEmpty();
        iconImage.color = Color.white;
        if (amount < 0.25f)
        {
            SetPartAsRefilling(progressImages[0], amount);
            iconImage.color = emptyColor;
        }
        else if (amount >= 0.25f && amount < 0.5f)
        {
            SetPartAsFilled(progressImages[0]);
            SetPartAsRefilling(progressImages[1], amount - 0.25f);
        }
        else if (amount >= 0.5f && amount < 0.75f)
        {
            SetPartAsFilled(progressImages[0]);
            SetPartAsFilled(progressImages[1]);
            SetPartAsRefilling(progressImages[2], amount - 0.5f);
        }
        else if (amount >= 0.75f && amount < 1f)
        {
            SetPartAsFilled(progressImages[0]);
            SetPartAsFilled(progressImages[1]);
            SetPartAsFilled(progressImages[2]);
            SetPartAsRefilling(progressImages[3], amount - 0.75f);
        }
        else
            SetAllAsFilled();
    }

    private void SetPartAsFilled(Image part)
    {
        part.color = FillColor;
        part.fillAmount = 0.25f;
    }

    private void SetAllAsFilled()
    {
        foreach (var part in progressImages)
            SetPartAsFilled(part);    
    }

    private void SetPartAsRefilling(Image part, float amount)
    {
        part.color = emptyColor;
        part.fillAmount = amount;
    }

    private void SetPartAsEmpty(Image part) => part.fillAmount = 0f;

    private void SetAllAsEmpty()
    {
        foreach (var part in progressImages)
            SetPartAsEmpty(part);
    }
}
