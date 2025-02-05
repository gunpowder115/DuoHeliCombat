using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float increaseScale = 1.5f;
    [SerializeField] private float decreaseSpeed = 1f;

    private float prevHP, fullHP;
    private float timeFromDamage;
    private float currScale, defaultScale, bigScale;
    private ScalableBar[] bars;
    private GameObject damageSource;

    private void Start()
    {
        bars = GetComponentsInChildren<ScalableBar>();
        prevHP = Mathf.Infinity;
        defaultScale = currScale = Mathf.Abs(transform.localScale.x);
        bigScale = defaultScale * increaseScale;
    }

    private void Update()
    {
        timeFromDamage += Time.deltaTime;
        currScale -= decreaseSpeed * Time.deltaTime;
        transform.localScale = new Vector3(-currScale, currScale, currScale);

        if (currScale <= defaultScale) currScale = defaultScale;

        if (damageSource)
        {
            transform.LookAt(damageSource.transform);
            Vector3 euler = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(0f, euler.y, 0f);
        }
    }

    public void SetHealth(float currHP)
    {
        if (currHP < prevHP)
        {
            timeFromDamage = 0f;
            prevHP = currHP;
            currScale = bigScale;
        }

        foreach (var bar in bars)
        {
            bar.SetScale(currHP / fullHP);
            bar.SetTimeFromDamage(timeFromDamage);
        }
    }

    public void SetFullHealth(float fullHP) => this.fullHP = fullHP;

    public void SetDamageSource(GameObject src) => damageSource = src;
}
