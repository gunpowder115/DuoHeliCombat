using System.Collections;
using UnityEngine;
using static Types;
using static ViewPortController;

public class RadialMenuController : MonoBehaviour
{
    [SerializeField] private float animationTime = 0.3f;
    [SerializeField] private float deltaX = 480f;
    [SerializeField] private float deltaY = 0f;
    [SerializeField] private Players playerNumber = Players.Player1;

    private float centerPosX, centerPosY;
    private CanvasGroup canvasGroup;
    private RadialMenuSelector radialMenuSelector;
    private ViewPortController viewPortController;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        radialMenuSelector = GetComponent<RadialMenuSelector>();
        gameObject.SetActive(false);

        viewPortController = ViewPortController.singleton;
        centerPosX = transform.position.x;
        centerPosY = transform.position.y;
    }

    public void ShowMenu()
    {
        if (playerNumber == Players.Player1)
        {
            if (viewPortController.SizeCamera1 == CameraSize.Half)
            {
                if (viewPortController.CameraOrientation == Orientation.Vertical)
                    transform.position = new Vector3(centerPosX - deltaX, transform.position.y, transform.position.z);
                else
                    transform.position = new Vector3(centerPosX, centerPosY + deltaY, transform.position.z);
            }
            else
                transform.position = new Vector3(centerPosX, centerPosY, transform.position.z);
        }
        else
        {
            if (viewPortController.SizeCamera2 == CameraSize.Half)
            {
                if (viewPortController.CameraOrientation == Orientation.Vertical)
                    transform.position = new Vector3(centerPosX + deltaX, transform.position.y, transform.position.z);
                else
                    transform.position = new Vector3(centerPosX, centerPosY - deltaY, transform.position.z);
            }
            else
                transform.position = new Vector3(centerPosX, centerPosY, transform.position.z);
        }

        gameObject.SetActive(true);
        radialMenuSelector.ResetSelectedIndex();
        StartCoroutine(AnimateMenu(0f, 1f));
    }

    public void HideMenu()
    {
        StartCoroutine(AnimateMenu(1f, 0f, () => gameObject.SetActive(false)));
    }

    private IEnumerator AnimateMenu(float startAlpha, float endAlpha, System.Action onComplete = null)
    {
        float t = 0f;
        Vector3 startScale = startAlpha == 0f ? Vector3.zero : Vector3.one;
        Vector3 endScale = startAlpha == 0f ? Vector3.one : Vector3.zero;
        transform.localScale = startScale;

        while (t < animationTime)
        {
            t += Time.deltaTime;
            float progress = t / animationTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
            transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        transform.localScale = endScale;
        onComplete?.Invoke();
    }
}
