using UnityEngine;

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

        #region Vertical Move

        playerInput.PlayerKeyboard.SingleUp.performed += context => SetVerticalMove(VerticalMoveDirection.SingleUp);
        playerInput.PlayerKeyboard.SingleDown.performed += context => SetVerticalMove(VerticalMoveDirection.SingleDown);
        playerInput.PlayerKeyboard.VertFastMod.performed += context => SetVerticalSingleFast();

        playerInput.PlayerKeyboard.SingleUp.canceled += context => CancelVerticalMove(VerticalMoveDirection.SingleUp);
        playerInput.PlayerKeyboard.SingleDown.canceled += context => CancelVerticalMove(VerticalMoveDirection.SingleDown);
        playerInput.PlayerKeyboard.VertFastMod.canceled += context => CancelVerticalSingleFast();

        #endregion
    }
}
