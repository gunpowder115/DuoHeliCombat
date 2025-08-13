using System.Collections;
using UnityEngine;

public class RadialMenuController : MonoBehaviour
{
    [SerializeField] private float animationTime = 0.3f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        ShowMenu();
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);
        StartCoroutine(AnimateMenu(0f, 1f));
    }

    public void HideMenu()
    {
        StartCoroutine(AnimateMenu(1f, 0f, () => gameObject.SetActive(false)));
    }

    IEnumerator AnimateMenu(float startAlpha, float endAlpha, System.Action onComplete = null)
    {
        float t = 0f;
        transform.localScale = Vector3.zero;

        while (t < animationTime)
        {
            t += Time.deltaTime;
            float progress = t / animationTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        transform.localScale = Vector3.one;
        onComplete?.Invoke();
    }
}
