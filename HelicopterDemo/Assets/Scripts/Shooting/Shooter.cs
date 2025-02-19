using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private float timeToNextLaunch = 0.5f;

    private int unguidedMissileIndex, guidedMissileIndex;
    private float currTimeFromLaunch;
    private List<BarrelLauncher> barrels;
    private List<MissileLauncher> unguidedMissiles, guidedMissiles;

    public void BarrelFire(GameObject target)
    {
        if (barrels.Count > 0)
        {
            foreach(var barrel in barrels)
                if (barrel) barrel.Fire(target);
        }
    }

    public void UnguidedMissileLaunch(GameObject target)
    {
        if (unguidedMissiles.Count > 0 && unguidedMissiles[unguidedMissileIndex].IsEnable)
            unguidedMissiles[unguidedMissileIndex].Launch(target);
    }

    public void GuidedMissileLaunch(GameObject target)
    {
        if (guidedMissiles.Count > 0 && guidedMissiles[guidedMissileIndex].IsEnable)
            guidedMissiles[guidedMissileIndex].Launch(target);
    }

    void Start()
    {
        barrels = new List<BarrelLauncher>();
        barrels.AddRange(GetComponentsInChildren<BarrelLauncher>());
        foreach (var bar in barrels)
            bar.gameObject.tag = gameObject.tag;

        List<MissileLauncher> missiles = new List<MissileLauncher>(GetComponentsInChildren<MissileLauncher>());
        unguidedMissiles = new List<MissileLauncher>();
        guidedMissiles = new List<MissileLauncher>();
        if (missiles != null)
        {
            foreach (var missile in missiles)
            {
                missile.gameObject.tag = gameObject.tag;
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
