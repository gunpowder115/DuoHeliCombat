using System.Collections;
using UnityEngine;
using static Types;

public class MissileLauncher : BaseLauncher
{
    public bool IsGuided => guided;
    public bool IsEnable => isEnable;

    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float rechargeTime = 5f;
    [SerializeField] private float shotDeltaTime = 0.5f;
    [SerializeField] private float maxClipVolume = 1f;
    [SerializeField] private float volumeRefillSpeed = 0.05f;
    [SerializeField] private bool guided = false;

    private bool isEnable, isRecharge;
    private float currShotDeltaTime;
    private float currVolume;
    private GameObject[] childObjects;

    public bool IsPlayer { get; set; }
    public GlobalSide2 Side { get; set; }
    public SingleProgressUI uiSingle { get; set; }
    private float NormClipVolume => currVolume / maxClipVolume;

    public void Launch(GameObject target)
    {
        if (currVolume >= maxClipVolume)
        {
            this.target = target;
            if (missilePrefab)
            {
                if (guided)
                {
                    var missileItem = Instantiate(missilePrefab, transform.position + transform.forward * 2f, transform.rotation);
                    GuidedMissile guidedMissile = missileItem.GetComponent<GuidedMissile>();
                    guidedMissile.Side = Side;
                    guidedMissile.IsPlayer = IsPlayer;
                    if (guidedMissile) guidedMissile.SelectedTarget = target;
                }
                else
                {
                    var missile = Instantiate(missilePrefab, transform.position + transform.forward * 2f, CalculateDeflection()).GetComponent<Projectile>();
                    missile.Side = Side;
                    missile.IsPlayer = IsPlayer;
                }
            }
            else
                Debug.Log(this.ToString() + ": missilePrefab is NULL!");
            SetMissileActive(false);
            uiSingle?.SetDisable();
            isRecharge = true;
            currVolume -= 1f;
            currShotDeltaTime = 0f;
        }
    }

    void Start()
    {
        currVolume = maxClipVolume;
        isEnable = true;
        childObjects = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            childObjects[i] = transform.GetChild(i).gameObject;
    }

    private void Update()
    {
        if (currVolume < maxClipVolume)
            currVolume += volumeRefillSpeed * Time.deltaTime;
        else
        {
            currVolume = maxClipVolume;
            uiSingle?.SetEnable();
            SetMissileActive(true);
            isRecharge = false;
        }
        uiSingle?.SetCircleAmount(NormClipVolume);
    }

    private void SetMissileActive(bool active)
    {
        foreach (var obj in childObjects)
            obj.SetActive(active);
        isEnable = active;
    }
}
