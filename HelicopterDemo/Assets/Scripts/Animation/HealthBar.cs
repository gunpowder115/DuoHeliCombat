using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private float prevHP, fullHP;
    private float timeFromDamage;
    private ScalableBar[] bars;

    private void Start()
    {
        bars = GetComponentsInChildren<ScalableBar>();
        prevHP = Mathf.Infinity;
    }

    private void Update()
    {
        timeFromDamage += Time.deltaTime;
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
}
