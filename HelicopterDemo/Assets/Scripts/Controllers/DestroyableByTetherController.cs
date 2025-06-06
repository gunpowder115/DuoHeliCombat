using Assets.Scripts.Gameplay.FuelWars;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableByTetherController
{
    private static DestroyableByTetherController singleton;
    public static DestroyableByTetherController Singleton
    {
        get
        {
            if (singleton == null)
                singleton = new DestroyableByTetherController();
            return singleton;
        }
    }

    public List<FuelTower> fuelTowers;
    public List<Walker> walkers;

    private DestroyableByTetherController()
    {
        fuelTowers = new List<FuelTower>();
        walkers = new List<Walker>();
    }

    public void AddItem(IDestroyableByTether item)
    {
        if (item is FuelTower && !fuelTowers.Contains(item as FuelTower))
            fuelTowers.Add(item as FuelTower);
        else if (item is Walker && !walkers.Contains(item as Walker))
            walkers.Add(item as Walker);
    }

    public void RemoveItem(IDestroyableByTether item)
    {
        if (item is FuelTower && fuelTowers.Contains(item as FuelTower))
            fuelTowers.Remove(item as FuelTower);
        else if (item is Walker && walkers.Contains(item as Walker))
            walkers.Remove(item as Walker);
    }

    public FuelTower GetNearFuelTower(in Vector3 origin, float maxDist)
    {
        foreach (var item in fuelTowers)
        {
            if (Mathf.Abs(origin.x - item.transform.position.x) < maxDist &&
                Mathf.Abs(origin.z - item.transform.position.z) < maxDist)
                return item;
        }
        return null;
    }

    public Walker GetNearWalker(in Vector3 origin, float maxDist)
    {
        foreach (var item in walkers)
        {
            if (Mathf.Abs(origin.x - item.transform.position.x) < maxDist &&
                Mathf.Abs(origin.z - item.transform.position.z) < maxDist)
                return item;
            else
            {
                item.StartWalker();
                item.SetRotation(null);
                item.StopFire();
            }
        }
        return null;
    }

    public void DestroyItem(IDestroyableByTether item, in Vector3 destroyDir)
    {
        RemoveItem(item);
        item.CallToDestroy(destroyDir);
    }
}