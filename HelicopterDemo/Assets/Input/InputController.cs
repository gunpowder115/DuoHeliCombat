using UnityEngine;
using static InputDeviceBase;

public class InputController : MonoBehaviour
{
    private InputKeyboard keyboard;
    private InputGamepad gamepad;

    public static InputController singleton { get; private set; }

    private void Awake()
    {
        singleton = this;

        keyboard = new InputKeyboard();
        keyboard.Init();

        gamepad = new InputGamepad();
        gamepad.Init();
    }

    private void OnEnable()
    {
        keyboard.OnEnable();
        gamepad.OnEnable();
    }

    private void OnDisable()
    {
        keyboard.OnDisable();
        gamepad.OnDisable();
    }

    public InputDeviceBase GetDevice(ControllerType controllerType)
    {
        switch (controllerType)
        {
            case ControllerType.Gamepad:
                return gamepad;
            default:
                return keyboard;
        }
    }
}
