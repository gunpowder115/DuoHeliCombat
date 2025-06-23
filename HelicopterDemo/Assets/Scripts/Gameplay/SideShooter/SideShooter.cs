using UnityEngine;
using static InputDeviceBase;
using static Types;

public class SideShooter : MonoBehaviour
{
    [SerializeField] private GlobalSide2 side = GlobalSide2.Blue;
    [SerializeField] ControllerType controllerType = ControllerType.Keyboard;

    private Shooter shooter;
    private InputDeviceBase inputDevice;
    private InputController inputController;

    private void Awake()
    {
        shooter = GetComponent<Shooter>();
        if (shooter) shooter.Side = side;
    }

    private void Start()
    {
        inputController = InputController.singleton;
        if (!inputController) return;
        inputDevice = inputController.GetDevice(controllerType);
    }

    private void Update()
    {
        if (inputDevice.MinigunFire)
            shooter.BarrelFire(null);
        else
            shooter.StopBarrelFire();
    }
}