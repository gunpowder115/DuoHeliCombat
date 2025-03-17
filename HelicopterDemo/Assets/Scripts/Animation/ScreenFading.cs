using UnityEngine;
using UnityEngine.UI;
using static Types;

public class ScreenFading : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float darkTime = 1f;

    private float currFullDarkTime;
    private Color color;
    private Image fadingImage;
    private FadingScreenType fadingScreenType;

    private void Start()
    {
        fadingImage = GetComponent<Image>();
        color = fadingImage.color;
        fadingScreenType = FadingScreenType.None;
    }

    private void Update()
    {
        switch(fadingScreenType)
        {
            case FadingScreenType.Darkening:
                if (color.a < 1f)
                {
                    color.a += speed * Time.deltaTime;
                    fadingImage.color = color;
                }
                else
                    fadingScreenType = FadingScreenType.FullDark;
                break;
            case FadingScreenType.FullDark:
                if (currFullDarkTime > darkTime)
                {
                    currFullDarkTime = 0f;
                    fadingScreenType = FadingScreenType.Lightening;
                }
                else
                    currFullDarkTime += Time.deltaTime;
                break;
            case FadingScreenType.Lightening:
                if (color.a > 0f)
                {
                    color.a -= speed * 2f * Time.deltaTime;
                    fadingImage.color = color;
                }
                else
                    fadingScreenType = FadingScreenType.None;
                break;
        }
    }

    public void StartFading()
    {
        if (fadingScreenType == FadingScreenType.None)
            fadingScreenType = FadingScreenType.Darkening;
    }
}
