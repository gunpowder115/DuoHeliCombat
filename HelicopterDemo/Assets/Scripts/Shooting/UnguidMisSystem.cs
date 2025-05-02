using System.Collections.Generic;
using UnityEngine;
using static Types;

public class UnguidMisSystem : BaseLauncher
{
    [SerializeField] private float maxVolume = 4f;
    [SerializeField] private float forceRechargeVolume = 1f;
    [SerializeField] private float volumeRefillSpeed = 5f;

    public bool IsPlayer { get; set; }
    public bool IsAvailableToLaunch => currMisIndex >= 0;
    private float NormClipVolume => currVolume / maxVolume;
    public GlobalSide2 Side { get; set; }
    public SingleProgressUI uiSingle { get; set; }

    private float currVolume;
    private int currMisIndex;
    private bool isForceRecharge;
    private List<MissileLauncher> unguidMissiles;

    private void Start()
    {
        unguidMissiles = new List<MissileLauncher>(GetComponentsInChildren<MissileLauncher>());
        foreach (var mis in unguidMissiles)
        {
            mis.Side = Side;
            mis.IsPlayer = IsPlayer;
        }

        currVolume = maxVolume;
        currMisIndex = (int)(maxVolume - 1f);
    }

    private void Update()
    {
        if (currVolume < forceRechargeVolume)
        {
            uiSingle?.SetDisable();
            isForceRecharge = true;
        }

        if (isForceRecharge)
            ForceRecharge();
        else
            Recharge();

        SetMissilesEnable();
    }

    public void Launch(GameObject target)
    {
        if (!isForceRecharge)
        {
            currVolume--;
            uiSingle?.SetCircleAmount(NormClipVolume);
            unguidMissiles[currMisIndex].Launch(target);
        }
    }

    private void Recharge()
    {
        if (currVolume < maxVolume)
        {
            currVolume += volumeRefillSpeed * Time.deltaTime;
            uiSingle?.SetCircleAmount(NormClipVolume);
        }
        else
            currVolume = maxVolume;
    }

    private void ForceRecharge()
    {
        if (currVolume < forceRechargeVolume)
        {
            currVolume += volumeRefillSpeed * Time.deltaTime;
            uiSingle?.SetCircleAmount(NormClipVolume);
        }
        else
        {
            currVolume = forceRechargeVolume;
            uiSingle?.SetEnable();
            isForceRecharge = false;
        }
    }

    private void SetMissilesEnable()
    {
        currMisIndex = Mathf.FloorToInt(currVolume) - 1;
        unguidMissiles[0].SetMissileActive(currMisIndex >= 0);
        unguidMissiles[1].SetMissileActive(currMisIndex >= 1);
        unguidMissiles[2].SetMissileActive(currMisIndex >= 2);
        unguidMissiles[3].SetMissileActive(currMisIndex >= 3);
    }
}
