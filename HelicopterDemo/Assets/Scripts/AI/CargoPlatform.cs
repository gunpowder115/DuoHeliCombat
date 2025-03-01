using UnityEngine;
using static Types;

[RequireComponent(typeof(Building))]

public class CargoPlatform : MonoBehaviour
{
    [SerializeField] private GameObject cargoHelicopterPrefab;
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private Caravan caravan;
    [SerializeField] private float dropHeight = 25f;
    [SerializeField] private CargoType cargoType = CargoType.Rope;

    private GameObject cargoHelicopterObject;
    private GameObject cargoObject;
    private CargoHelicopter cargoHelicopter;
    private NpcController npcController;
    private CargoState cargoState;

    public float DropHeight => dropHeight;
    public Caravan Caravan => caravan;

    private void Start()
    {
        npcController = NpcController.Singleton;
        cargoState = CargoState.Lost;
    }

    // Update is called once per frame
    void Update()
    {
        switch(cargoType)
        {
            case CargoType.Squad:
                CallGroundCargo();
                break;
            case CargoType.Rope:
                CallAirCargo();
                break;
        }
    }

    private void CallGroundCargo()
    {
        switch (cargoState)
        {
            case CargoState.Works:
                if (cargoObject.gameObject == null)
                    cargoState = CargoState.Lost;
                break;
            case CargoState.Lost:
                cargoHelicopterObject = Instantiate(cargoHelicopterPrefab, transform.position, transform.rotation);
                cargoHelicopter = cargoHelicopterObject.GetComponent<CargoHelicopter>();
                cargoHelicopter.InitForDrop(transform.position, dropHeight);
                cargoState = CargoState.Expecting;
                break;
            case CargoState.Expecting:
                if (cargoHelicopter.NearDropPoint)
                {
                    cargoObject = Instantiate(cargoPrefab, gameObject.transform.position, gameObject.transform.rotation);
                    if (!cargoObject.GetComponent<NpcSquad>())
                        npcController.Add(cargoObject);
                    CargoItem cargoItem = cargoObject.GetComponent<CargoItem>();
                    if (cargoItem)
                        cargoItem.Init(this);
                    cargoState = CargoState.Works;
                }
                break;
        }
    }

    private void CallAirCargo()
    {
        switch (cargoState)
        {
            case CargoState.Works:
                if (cargoObject.gameObject == null)
                    cargoState = CargoState.Lost;
                break;
            case CargoState.Lost:
                cargoHelicopterObject = Instantiate(cargoHelicopterPrefab, transform.position, transform.rotation);
                cargoHelicopter = cargoHelicopterObject.GetComponent<CargoHelicopter>();
                cargoHelicopter.InitForDelivery(transform.position, dropHeight);

                cargoObject = Instantiate(cargoPrefab, cargoHelicopter.transform.position - new Vector3(0f, cargoHelicopter.CableLength, 0f), 
                    cargoHelicopter.transform.rotation, cargoHelicopter.transform);
                CargoItem cargoItem_ = cargoObject.GetComponent<CargoItem>();
                if (cargoItem_)
                    cargoItem_.Init(this);
                cargoState = CargoState.Expecting;
                break;
            case CargoState.Expecting:
                if (cargoHelicopter.NearDropPoint)
                {
                    Destroy(cargoObject);
                    cargoObject = Instantiate(cargoPrefab, transform.position, transform.rotation);
                    if (!cargoObject.GetComponent<NpcSquad>())
                        npcController.Add(cargoObject);
                    CargoItem cargoItem = cargoObject.GetComponent<CargoItem>();
                    if (cargoItem)
                        cargoItem.Init(this);
                    cargoState = CargoState.Works;
                }
                break;
        }
    }

    public enum CargoState
    {
        Works,
        Lost,
        Expecting,
        Delivered
    }
}
