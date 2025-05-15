using UnityEngine;

public class FuelTower : MonoBehaviour
{
    [SerializeField] private ParticleSystem destroyParticleSystem;
    [SerializeField] private GameObject fuelTowerDestroyedPrefab;

    private FuelTowersController fuelTowersController;

    private void Awake()
    {
        fuelTowersController = FuelTowersController.Singleton;
        fuelTowersController.AddFuelTower(this);
    }

    public void CallToDestroy()
    {
        destroyParticleSystem.Play();
        Destroy(gameObject);
        Instantiate(fuelTowerDestroyedPrefab);
    }
}
