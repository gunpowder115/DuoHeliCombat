using UnityEngine;
using static Types;

public class BuildingSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] redBuildingsPrefabs;
    [SerializeField] private GameObject[] blueBuildingsPrefabs;
    [SerializeField] private GameObject[] redCargosPrefabs;
    [SerializeField] private GameObject[] blueCargosPrefabs;
    [SerializeField] private GameObject redCargoHelicopterPrefab;
    [SerializeField] private GameObject blueCargoHelicopterPrefab;

    private bool isDeliveryWaiting;
    private CargoHelicopter cargoHelicopter;
    private GameObject buildingPrefab, cargoPrefab;
    private Platform platform;
    private PlatformController platformController;

    private void Start()
    {
        isDeliveryWaiting = false;
        platform = GetComponent<Platform>();
        platformController = PlatformController.Singleton;
    }

    private void Update()
    {
        if (isDeliveryWaiting && cargoHelicopter && cargoHelicopter.CargoIsDelivered)
        {
            platform.InitBuilding(buildingPrefab);
            platform.IsReserved = false;
            isDeliveryWaiting = false;
        }
    }

    public void CallBuilding(int buildNumber, GlobalSide2 side)
    {
        buildingPrefab = side == GlobalSide2.Blue ? blueBuildingsPrefabs[buildNumber - 1] : redBuildingsPrefabs[buildNumber - 1];
        cargoPrefab = side == GlobalSide2.Blue ? blueCargosPrefabs[buildNumber - 1] : redCargosPrefabs[buildNumber - 1];

        if (!cargoHelicopter)
        {
            cargoHelicopter = Instantiate(side == GlobalSide2.Blue ? blueCargoHelicopterPrefab : redCargoHelicopterPrefab, transform.position, transform.rotation).GetComponent<CargoHelicopter>();
            cargoHelicopter.Init(cargoPrefab, transform.position, CargoType.Dropping);
            isDeliveryWaiting = true;
            platform.ShowPlatform();
            platform.IsReserved = true;
            platformController.Remove(gameObject);
        }
    }
}
