using System;
using UnityEngine;

public class CargoItem : MonoBehaviour
{
    [SerializeField] private float deliverySpeed = 5f;
    [SerializeField] private GameObject parachutePrefab;

    public float DeliverySpeed => deliverySpeed;
    public float DropHeight => CargoPlatform.DropHeight;
    public float ParachuteHeight => CargoPlatform.ParachuteHeight;
    public GameObject ParachutePrefab => parachutePrefab;
    public CargoPlatform CargoPlatform { get; private set; }
    public Action<Caravan> InitCargoItem { get; set;  }

    public void Init(CargoPlatform cargoPlatform)
    {
        CargoPlatform = cargoPlatform;
        InitCargoItem?.Invoke(cargoPlatform.Caravan);
    }
}
