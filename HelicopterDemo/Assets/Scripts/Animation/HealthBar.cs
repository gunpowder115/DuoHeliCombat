using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private float prevHP, fullHP;
    private float timeFromDamage;
    private ScalableBar[] bars;
    private GameObject damageSource;

    private void Start()
    {
        bars = GetComponentsInChildren<ScalableBar>();
        prevHP = Mathf.Infinity;
    }

    private void Update()
    {
        timeFromDamage += Time.deltaTime;

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
