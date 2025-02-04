using UnityEngine;

public class ScalableBar : MonoBehaviour
{
    [SerializeField] private bool isEmpty = false;

    private FadingOut fadingOut;

    private void Start()
    {
        fadingOut = GetComponent<FadingOut>();
    }

    public void SetScale(float hp)
    {
        Vector3 currScale = transform.localScale;

        if (hp < 0f) hp = 0f;
        else if (hp > 1f) hp = 1f;

        if (isEmpty)
            transform.localScale = new Vector3(1f - hp, currScale.y, currScale.z);
        else
            transform.localScale = new Vector3(hp, currScale.y, currScale.z);
    }

    public void SetTimeFromDamage(float time) => fadingOut.CurrTime = time;
}
