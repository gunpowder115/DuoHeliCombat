using UnityEngine;
using UnityEngine.UI;

public class SingleProgressUI : MonoBehaviour
{
    [SerializeField] private bool useInvert = false;
    [SerializeField] private bool mirrorIcon = false;
    [SerializeField] private bool scaleIfDecreased = false;
    [SerializeField] private float scaleSpeed = 10f;
    [SerializeField] private float scaleCoef = 1.2f;
    [SerializeField] private Image progressImage;
    [SerializeField] private Image invertProgressImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Color fillColor = Color.gray;
    [SerializeField] private Color emptyColor = Color.red;

    private bool isScaling;
    private float baseScale;

    private void Start()
    {
        SetFillColor();
        if (useInvert)
        {
            invertProgressImage.color = emptyColor;
            invertProgressImage.fillAmount = 0f;
        }
        if (mirrorIcon)
            iconImage.rectTransform.localScale = new Vector3(-iconImage.rectTransform.localScale.x,
                                                            iconImage.rectTransform.localScale.y,
                                                            iconImage.rectTransform.localScale.z);

        baseScale = transform.localScale.x;
    }

    private void Update()
    {
        if (isScaling)
        {
            float scale = transform.localScale.x - scaleSpeed * Time.deltaTime;
            if (scale <= baseScale)
            {
                scale = baseScale;
                isScaling = false;
            }
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void SetCircleAmount(float amount)
    {
        if (scaleIfDecreased && amount < progressImage.fillAmount)
        {
            isScaling = true;
            transform.localScale = new Vector3(baseScale, baseScale, baseScale) * scaleCoef;
        }

        progressImage.fillAmount = amount;
        if (useInvert)
            invertProgressImage.fillAmount = 1 - amount;
    }

    public void SetFillColor() => progressImage.color = fillColor;
    public void SetEmptyColor() => progressImage.color = emptyColor;
}