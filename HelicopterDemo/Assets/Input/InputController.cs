using UnityEngine;
using static InputDeviceBase;

public class InputController : MonoBehaviour
{
    private InputKeyboard keyboard;
    private InputGamepad gamepad;
    private InputCommon inputCommon;

    public static InputController singleton { get; private set; }
    public InputDeviceBase GetInputCommon => inputCommon;

    private void Awake()
    {
        singleton = this;

        keyboard = new InputKeyboard();
        keyboard.Init();

        gamepad = new InputGamepad();
        gamepad.Init();

        inputCommon = new InputCommon();
        inputCommon.Init();
    }

    private void OnEnable()
    {
        keyboard.OnEnable();
        gamepad.OnEnable();
        inputCommon.OnEnable();
    }

    private void OnDisable()
    {
        keyboard.OnDisable();
        gamepad.OnDisable();
        inputCommon.OnDisable();
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
