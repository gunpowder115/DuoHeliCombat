using UnityEngine;
using UnityEngine.UI;

public class QuadroProgressUI : MonoBehaviour
{
    [SerializeField] private Image[] progressImages;
    [SerializeField] private Color fillColor = Color.gray;
    [SerializeField] private Color emptyColor = Color.red;

    private void Start()
    {
        SetAllAsFilled();
    }

    public void SetCircleAmount(float amount)
    {
        SetAllAsEmpty();
        if (amount < 0.25f)
        {
            SetPartAsRefilling(progressImages[0], amount);
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
        part.color = fillColor;
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
