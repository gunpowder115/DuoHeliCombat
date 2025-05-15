using UnityEngine;

public class FuelTower : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffectPrefab;
    [SerializeField] private GameObject fuelTowerDestroyedPrefab;

    private FuelTowersController fuelTowersController;

    private void Awake()
    {
        fuelTowersController = FuelTowersController.Singleton;
        fuelTowersController.AddFuelTower(this);
    }

    public void CallToDestroy()
    {
        if (destroyEffectPrefab) Instantiate(destroyEffectPrefab);
        Destroy(gameObject);
        if (fuelTowerDestroyedPrefab) Instantiate(fuelTowerDestroyedPrefab);
    }
}
