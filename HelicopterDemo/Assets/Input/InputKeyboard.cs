using UnityEngine;
using static Types;

public class InputKeyboard : InputDeviceBase
{
    public override Vector2 GetInput() => playerInput.PlayerKeyboard.Move.ReadValue<Vector2>();

    public override Vector2 GetCameraInput() => playerInput.CameraKeyboard.Move.ReadValue<Vector2>();

    public void OnEnable() => playerInput.Enable();

    public void OnDisable() => playerInput.Disable();

    new public void Init()
    {
        base.Init();
        controllerItem = this;

        playerInput.CommonKeyboard.MainAction.performed += context => DoMainAction();
        playerInput.CommonKeyboard.MainAction.canceled += context => DoMainActionCancel();

        playerInput.CommonKeyboard.MinorAction.performed += context => DoMinorAction();
        playerInput.CommonKeyboard.MinorActionHold.performed += context => DoMinorActionHold();

        playerInput.CommonKeyboard.AnyTargetSelection.performed += context => AnyTargetSelection();
        playerInput.CommonKeyboard.AnyTargetSelection.canceled += context => AnyTargetSelectionCancel();

        playerInput.PlayerKeyboard.FastMove.performed += context => FastMove();
        playerInput.PlayerKeyboard.FastMove.canceled += context => FastMoveCancel();

        playerInput.CommonKeyboard.Take.performed += context => Take();

        playerInput.CommonKeyboard.Rescue.performed += context => Rescue();

        #region Vertical Move

        playerInput.PlayerKeyboard.SingleUp.performed += context => SetVerticalMove(VerticalMoveDirection.SingleUp);
        playerInput.PlayerKeyboard.SingleDown.performed += context => SetVerticalMove(VerticalMoveDirection.SingleDown);

        playerInput.PlayerKeyboard.SingleUp.canceled += context => CancelVerticalMove(VerticalMoveDirection.SingleUp);
        playerInput.PlayerKeyboard.SingleDown.canceled += context => CancelVerticalMove(VerticalMoveDirection.SingleDown);

        #endregion

        #region Building Selection

        playerInput.CommonKeyboard.Build_1.performed += context => SelectBuilding(1, GlobalSide2.Blue);
        playerInput.CommonKeyboard.Build_2.performed += context => SelectBuilding(2, GlobalSide2.Blue);
        playerInput.CommonKeyboard.Build_3.performed += context => SelectBuilding(3, GlobalSide2.Blue);
        playerInput.CommonKeyboard.Build_4.performed += context => SelectBuilding(4, GlobalSide2.Blue);
        playerInput.CommonKeyboard.Build_5.performed += context => SelectBuilding(5, GlobalSide2.Blue);
        playerInput.CommonKeyboard.Build_6.performed += context => SelectBuilding(6, GlobalSide2.Blue);

        playerInput.CommonKeyboard.AltBuild_1.performed += context => SelectBuilding(1, GlobalSide2.Red);
        playerInput.CommonKeyboard.AltBuild_2.performed += context => SelectBuilding(2, GlobalSide2.Red);
        playerInput.CommonKeyboard.AltBuild_3.performed += context => SelectBuilding(3, GlobalSide2.Red);
        playerInput.CommonKeyboard.AltBuild_4.performed += context => SelectBuilding(4, GlobalSide2.Red);
        playerInput.CommonKeyboard.AltBuild_5.performed += context => SelectBuilding(5, GlobalSide2.Red);
        playerInput.CommonKeyboard.AltBuild_6.performed += context => SelectBuilding(6, GlobalSide2.Red);

        #endregion
    }
}
