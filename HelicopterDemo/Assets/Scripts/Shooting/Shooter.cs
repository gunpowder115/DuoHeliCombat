using System.Collections.Generic;
using UnityEngine;
using static Types;

public class Shooter : MonoBehaviour
{
    [SerializeField] private float timeToNextLaunch = 0.5f;

    private int guidedMissileIndex;
    private float currTimeFromLaunch;
    private List<BarrelLauncher> barrels;
    private List<MissileLauncher> guidedMissiles;
    private List<UnguidMisSystem> unguidMisSystems;

    public bool IsPlayer { get; private set; }
    public GlobalSide2 Side { get; set; }

    public void BarrelFire(GameObject target)
    {
        if (barrels.Count > 0)
        {
            foreach(var barrel in barrels)
                if (barrel) barrel.Fire(target);
        }
    }

    public void StopBarrelFire()
    {
        if (barrels.Count > 0)
        {
            foreach (var barrel in barrels)
                if (barrel) barrel.StopFire();
        }
    }

    public bool UnguidedMissileLaunch(GameObject target)
    {
        foreach (var misSys in unguidMisSystems)
        {
            if (misSys.IsAvailableToLaunch)
            {
                misSys.Launch(target);
                return true;
            }
        }
        if (unguidMisSystems.Count > 0) 
            unguidMisSystems[0].uiSingle?.ScaleAndSound();
        return false;
    }

    public bool GuidedMissileLaunch(GameObject target)
    {
        if (guidedMissiles.Count > 0 && guidedMissiles[guidedMissileIndex].IsEnable)
        {
            guidedMissiles[guidedMissileIndex].Launch(target);
            return true;
        }
        if (guidedMissiles.Count > 0)
            guidedMissiles[0].uiSingle?.ScaleAndSound();
        return false;
    }

    void Start()
    {
        IsPlayer = GetComponent<Player>();
        barrels = new List<BarrelLauncher>();
        barrels.AddRange(GetComponentsInChildren<BarrelLauncher>());
        foreach (var bar in barrels)
        {
            bar.Side = Side;
            bar.IsPlayer = IsPlayer;
        }

        List<MissileLauncher> missiles = new List<MissileLauncher>(GetComponentsInChildren<MissileLauncher>());
        guidedMissiles = new List<MissileLauncher>();
        if (missiles != null)
        {
            foreach (var missile in missiles)
            {
                missile.Side = Side;
                missile.IsPlayer = IsPlayer;
                if (missile.IsGuided)
                    guidedMissiles.Add(missile);
            }
        }

        unguidMisSystems = new List<UnguidMisSystem>(GetComponentsInChildren<UnguidMisSystem>());
        foreach (var misSys in unguidMisSystems)
        {
            misSys.Side = Side;
            misSys.IsPlayer = IsPlayer;
        }
    }

    private void Update()
    {
        currTimeFromLaunch += Time.deltaTime;
        if (currTimeFromLaunch > timeToNextLaunch)
        {
            currTimeFromLaunch = 0f;
            if (++guidedMissileIndex >= guidedMissiles.Count) guidedMissileIndex = 0;
        }
    }
}
