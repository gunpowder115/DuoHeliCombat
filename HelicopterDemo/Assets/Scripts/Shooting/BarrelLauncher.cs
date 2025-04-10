using UnityEngine;
using static Types;

public class BarrelLauncher : BaseLauncher
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject flashPrefab;
    [SerializeField] float shotDeltaTime = 0.5f;
    [SerializeField] float rechargeTime = 5f;
    [SerializeField] int maxClipVolume = 1;

    int currClipVolume;
    float currShotDeltaTime, currRechargeTime;
    private float tgtShortDeltaTime;

    public GlobalSide2 Side { get; set; }

    public void Fire(GameObject target)
    {
        this.target = target;
        if (currClipVolume >= 0f)
            Shoot();
        else
            Recharge();
    }

    void Start()
    {
        currClipVolume = maxClipVolume;
    }

    void Shoot()
    {
        if (currShotDeltaTime >= tgtShortDeltaTime)
        {
            if (projectilePrefab && flashPrefab)
            {
                GameObject flash = Instantiate(flashPrefab, transform);
                Destroy(flash, 0.05f);

                var proj = Instantiate(projectilePrefab, this.transform.position, CalculateDeflection()).GetComponent<Projectile>();
                proj.Side = Side;
            }
            else
                Debug.Log(this.ToString() + ": projectilePrefab is NULL!");

            if (maxClipVolume > 0f) currClipVolume--;
            currShotDeltaTime = 0f;
            tgtShortDeltaTime = Random.Range(shotDeltaTime, shotDeltaTime * 1.5f);
        }
        else
            currShotDeltaTime += Time.deltaTime;
    }

    void Recharge()
    {
        if (currRechargeTime >= rechargeTime)
        {
            currClipVolume = maxClipVolume;
            currRechargeTime = 0f;
        }
        else
            currRechargeTime += Time.deltaTime;
    }
}
