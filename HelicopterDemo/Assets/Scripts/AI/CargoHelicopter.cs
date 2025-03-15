using System.Collections.Generic;
using UnityEngine;
using static Types;

[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]

public class CargoHelicopter : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float distance = 80f;
    [SerializeField] private float leaveDistCoef = 1.1f;
    [SerializeField] private float dropDistDelta = 0.5f;
    [SerializeField] private float cableLength = 5f;
    [SerializeField] private float height = 12f;

    public bool CargoIsDelivered { get; private set; }
    public bool NearDropPoint => currDist < dropDistDelta;
    public float CableLength => cableLength;

    private bool isEscape;
    private float currDist;
    private Vector3 deliveryPoint, escapePoint;
    private Translation translation;
    private Rotation rotation;
    private List<SimpleRotor> rotors;

    private CargoType cargoType;
    private Vector3 cargoPoint;
    private GameObject cargoItem, cargoPrefab;

    private void Awake()
    {
        translation = GetComponent<Translation>();
        rotation = GetComponent<Rotation>();
        rotors = new List<SimpleRotor>();
        rotors.AddRange(GetComponentsInChildren<SimpleRotor>());
        foreach (var rotor in rotors)
            rotor.FastStartRotor();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toDeliveryPoint = deliveryPoint - transform.position;
        Vector3 toEscapePoint = (escapePoint - transform.position).normalized;
        currDist = toDeliveryPoint.magnitude;
        toDeliveryPoint = toDeliveryPoint.normalized;
        float currentSpeed = speed;

        if (cargoType == CargoType.Dropping)
        {
            if (isEscape)
                translation.SetGlobalTranslation(toEscapePoint * currentSpeed);
            else
            {
                translation.SetGlobalTranslation(toDeliveryPoint * currentSpeed);
                if (NearDropPoint && !isEscape && !cargoItem && cargoPrefab)
                {
                    cargoItem = Instantiate(cargoPrefab, deliveryPoint, transform.rotation);
                    isEscape = true;
                }
            }

            if (cargoItem && cargoItem.transform.position.y < cargoPoint.y)
            {
                CargoIsDelivered = true;
                Destroy(cargoItem);
            }

            if (currDist > distance * leaveDistCoef && isEscape)
                Destroy(gameObject);
        }
        else if (cargoType == CargoType.Delivering)
        {
            if (isEscape)
                translation.SetGlobalTranslation(toEscapePoint * currentSpeed);
            else
            {
                translation.SetGlobalTranslation(toDeliveryPoint * currentSpeed);
                if (!isEscape && !cargoItem && cargoPrefab)
                {
                    cargoItem = Instantiate(cargoPrefab, transform);
                }
            }

            if (cargoItem && NearDropPoint)
            {
                CargoIsDelivered = true;
                isEscape = true;
                Destroy(cargoItem);
            }

            if (currDist > distance * leaveDistCoef && isEscape)
                Destroy(gameObject);
        }
    }

    public void Init(GameObject cargoPrefab, Vector3 cargoPlatformPos, CargoType cargoType)
    {
        this.cargoType = cargoType;
        this.cargoPrefab = cargoPrefab;
        cargoPoint = cargoPlatformPos;

        switch (cargoType)
        {
            case CargoType.Delivering:
                deliveryPoint = new Vector3(cargoPlatformPos.x, cargoPlatformPos.y + cableLength, cargoPlatformPos.z);
                currDist = Random.Range(0.8f * distance, 1.2f * distance);
                transform.Translate(0, height + cableLength, -currDist);
                escapePoint = transform.position + transform.forward * 2.5f * distance;
                break;
            case CargoType.Dropping:
                deliveryPoint = new Vector3(cargoPlatformPos.x, cargoPlatformPos.y + height, cargoPlatformPos.z);
                currDist = Random.Range(0.8f * distance, 1.2f * distance);
                transform.Translate(0, height, -currDist);
                escapePoint = transform.position + transform.forward * 2.5f * distance;
                break;
        }
    }
}
