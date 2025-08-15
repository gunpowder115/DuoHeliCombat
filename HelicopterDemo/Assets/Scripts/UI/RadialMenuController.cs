using System.Collections;
using UnityEngine;

public class RadialMenuController : MonoBehaviour
{
    [SerializeField] private float animationTime = 0.3f;

    private CanvasGroup canvasGroup;
    private RadialMenuSelector radialMenuSelector;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        radialMenuSelector = GetComponent<RadialMenuSelector>();
        gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
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
