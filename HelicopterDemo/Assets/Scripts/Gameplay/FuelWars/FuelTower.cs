using Assets.Scripts.Gameplay.FuelWars;
using UnityEngine;

public class FuelTower : MonoBehaviour, IDestroyableByTether
{
    [SerializeField] private GameObject destroyEffectPrefab;
    [SerializeField] private GameObject fuelTowerDestroyedPrefab;

    private DestroyableByTetherController destroyableByTetherController;

    private void Awake()
    {
        destroyableByTetherController = DestroyableByTetherController.Singleton;
        destroyableByTetherController.AddItem(this);
    }

    public void CallToDestroy(in Vector3 destroyDir)
    {
        if (destroyEffectPrefab) Instantiate(destroyEffectPrefab, gameObject.transform.position, gameObject.transform.rotation);
        if (fuelTowerDestroyedPrefab) Instantiate(fuelTowerDestroyedPrefab, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(gameObject);
    }
}
