using UnityEngine;

public class InputCommon : InputDeviceBase
{
    public override Vector2 GetCameraInput()
    {
        throw new System.NotImplementedException();
    }

    public override Vector2 GetInput()
    {
        throw new System.NotImplementedException();
    }

    public void OnEnable() => playerInput.Enable();

    public void OnDisable() => playerInput.Disable();

    new public void Init()
    {
        base.Init();
        controllerItem = this;

        playerInput.ViewportDebug.ChangePlayer1.performed += context => SwitchPlayerConnection(1);
        playerInput.ViewportDebug.ChangePlayer2.performed += context => SwitchPlayerConnection(2);
        playerInput.ViewportDebug.ChangeConfiguration.performed += context => SwitchConfiguration();
        playerInput.ViewportDebug.ChangeOrientation.performed += context => SwitchOrientation();

        playerInput.Upgrade.WeaponUp.performed += context => WeaponUp();
        playerInput.Upgrade.WeaponDown.performed += context => WeaponDown();
        playerInput.Upgrade.SlotLeft.performed += context => SlotLeft();
        playerInput.Upgrade.SlotRight.performed += context => SlotRight();
        playerInput.Upgrade.WingUp.performed += context => WingUp();
        playerInput.Upgrade.WingDown.performed += context => WingDown();
    }
}
