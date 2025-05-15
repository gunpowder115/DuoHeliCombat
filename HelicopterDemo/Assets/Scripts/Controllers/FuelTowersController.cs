using System.Collections.Generic;
using UnityEngine;

public class FuelTowersController
{
    private static FuelTowersController singleton;
    public static FuelTowersController Singleton
    {
        get
        {
            if (singleton == null)
                singleton = new FuelTowersController();
            return singleton;
        }
    }

    public List<FuelTower> fuelTowers;

    private FuelTowersController()
    {
        fuelTowers = new List<FuelTower>();
    }

    public void AddFuelTower(FuelTower tower)
    {
        if (!fuelTowers.Contains(tower))
            fuelTowers.Add(tower);
    }

    public void RemoveFuelTower(FuelTower tower)
    {
        if (fuelTowers.Contains(tower))
            fuelTowers.Remove(tower);
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

    public void DestroyFuelTower(FuelTower tower)
    {
        RemoveFuelTower(tower);
        tower.CallToDestroy();
    }
}
