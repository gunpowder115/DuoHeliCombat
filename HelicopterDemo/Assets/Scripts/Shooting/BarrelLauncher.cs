using UnityEngine;
using static Types;

public class BarrelLauncher : BaseLauncher
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject flashPrefab;
    [SerializeField] private CircleProgress uiCircle;
    [SerializeField] private float shotDeltaTime = 0.5f;
    [SerializeField] private float rechargeTime = 5f;
    [SerializeField] private float maxClipVolume = 10f;
    [SerializeField] private float minClipVolumeToRefill = 1f;
    [SerializeField] private float maxClipVolumeToRefill = 1f;
    [SerializeField] private float volumeRefillSpeed = 0.05f;

    private bool isFire, isForceRecharge;
    private float currClipVolume;
    private float currShotDeltaTime;
    private float tgtShortDeltaTime;

    public bool IsPlayer { get; set; }
    public GlobalSide2 Side { get; set; }
    private float NormClipVolume => currClipVolume / maxClipVolume;

    public void Fire(GameObject target)
    {
        this.target = target;
        isFire = !isForceRecharge;
    }

    public void StopFire() => isFire = false;

    void Start()
    {
        currClipVolume = maxClipVolume;
    }

    private void Update()
    {
        if (currClipVolume < minClipVolumeToRefill)
        {
            uiCircle?.SetEmptyColor();
            isForceRecharge = true;
        }

        if (isForceRecharge)
            ForceRecharge();
        else if (isFire)
            Shoot();
        else
            Recharge();
    }

    private void Shoot()
    {
        if (currShotDeltaTime >= tgtShortDeltaTime)
        {
            if (projectilePrefab && flashPrefab)
            {
                GameObject flash = Instantiate(flashPrefab, transform);
                Destroy(flash, 0.05f);

                var proj = Instantiate(projectilePrefab, this.transform.position, CalculateDeflection()).GetComponent<Projectile>();
                proj.Side = Side;
                proj.IsPlayer = IsPlayer;
            }
            else
                Debug.Log(this.ToString() + ": projectilePrefab is NULL!");

            if (maxClipVolume > 0f)
            {
                currClipVolume--;
                uiCircle?.SetCircleAmount(NormClipVolume);
            }
            currShotDeltaTime = 0f;
            tgtShortDeltaTime = Random.Range(shotDeltaTime, shotDeltaTime * 1.5f);
        }
        else
            currShotDeltaTime += Time.deltaTime;
    }

    private void Recharge()
    {
        if (currClipVolume < maxClipVolume)
        {
            currClipVolume += volumeRefillSpeed * Time.deltaTime;
            uiCircle?.SetCircleAmount(NormClipVolume);
        }
        else
            currClipVolume = maxClipVolume;
    }

    private void ForceRecharge()
    {
        if (currClipVolume < maxClipVolumeToRefill)
        {
            currClipVolume += volumeRefillSpeed * Time.deltaTime;
            uiCircle?.SetCircleAmount(NormClipVolume);
        }
        else
        {
            currClipVolume = maxClipVolumeToRefill;
            uiCircle?.SetFillColor();
            isForceRecharge = false;
        }
    }
}
