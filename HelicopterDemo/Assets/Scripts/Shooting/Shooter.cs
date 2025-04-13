using System.Collections.Generic;
using UnityEngine;
using static Types;

public class Shooter : MonoBehaviour
{
    [SerializeField] private float timeToNextLaunch = 0.5f;

    private int unguidedMissileIndex, guidedMissileIndex;
    private float currTimeFromLaunch;
    private List<BarrelLauncher> barrels;
    private List<MissileLauncher> unguidedMissiles, guidedMissiles;

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

    public bool UnguidedMissileLaunch(GameObject target)
    {
        if (unguidedMissiles.Count > 0 && unguidedMissiles[unguidedMissileIndex].IsEnable)
        {
            unguidedMissiles[unguidedMissileIndex].Launch(target);
            return true;
        }
        return false;
    }

    public bool GuidedMissileLaunch(GameObject target)
    {
        if (guidedMissiles.Count > 0 && guidedMissiles[guidedMissileIndex].IsEnable)
        {
            guidedMissiles[guidedMissileIndex].Launch(target);
            return true;
        }
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
        unguidedMissiles = new List<MissileLauncher>();
        guidedMissiles = new List<MissileLauncher>();
        if (missiles != null)
        {
            foreach (var missile in missiles)
            {
                missile.Side = Side;
                missile.IsPlayer = IsPlayer;
                if (missile.IsGuided)
                    guidedMissiles.Add(missile);
                else
                    unguidedMissiles.Add(missile);
            }
        }
    }

    private void Update()
    {
        currTimeFromLaunch += Time.deltaTime;
        if (currTimeFromLaunch > timeToNextLaunch)
        {
            currTimeFromLaunch = 0f;

            if (++unguidedMissileIndex >= unguidedMissiles.Count) unguidedMissileIndex = 0;
            if (++guidedMissileIndex >= guidedMissiles.Count) guidedMissileIndex = 0;
        }
    }
}
