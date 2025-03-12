using UnityEngine;
using static Types;

public class CargoPlatform_new : MonoBehaviour
{
    [SerializeField] private float cargoOffsetY = 0f;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private GameObject redCargoHelicopterPrefab;
    [SerializeField] private GameObject blueCargoHelicopterPrefab;
    [SerializeField] private CargoType cargoType = CargoType.OneParachute;

    private bool isDeliveryWaiting;
    private CargoHelicopter cargoHelicopter;
    private GameObject item, cargoHelicopterPrefab;
    private Building building;
    private Vector3 cargoPosition;
    private GlobalSide2 side;

    private void Start()
    {
        isDeliveryWaiting = false;
        building = GetComponent<Building>();
        side = building.BuildingSide;
        cargoHelicopterPrefab = side == GlobalSide2.Blue ? blueCargoHelicopterPrefab : redCargoHelicopterPrefab;
    }

    private void Update()
    {
        cargoPosition = transform.position + new Vector3(0f, cargoOffsetY, 0f);

        if (isDeliveryWaiting && cargoHelicopter && cargoHelicopter.CargoIsDelivered)
        {
            item = Instantiate(itemPrefab, cargoPosition, transform.rotation);
            isDeliveryWaiting = false;
        }

        if (!item && !isDeliveryWaiting)
            CallCargo();
    }

    public void CallCargo()
    {
        if (!cargoHelicopter)
        {
            cargoHelicopter = Instantiate(cargoHelicopterPrefab, cargoPosition, transform.rotation).GetComponent<CargoHelicopter>();
            cargoHelicopter.Init(cargoPrefab, cargoPosition, cargoType);
            isDeliveryWaiting = true;
        }
    }
}
