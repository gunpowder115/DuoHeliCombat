using UnityEngine;

public class InputGamepad : InputDeviceBase
{
    public override Vector2 GetInput() => playerInput.PlayerGamepad.Move.ReadValue<Vector2>();

    public override Vector2 GetCameraInput() => playerInput.CameraGamepad.Move.ReadValue<Vector2>();

    public void OnEnable() => playerInput.Enable();

    public void OnDisable() => playerInput.Disable();

    new public void Init()
    {
        base.Init();
        controllerItem = this;

        playerInput.CommonGamepad.MainAction.performed += context => DoMainAction();
        playerInput.CommonGamepad.MainAction.canceled += context => DoMainActionCancel();

        playerInput.CommonGamepad.MinorAction.performed += context => DoMinorAction();
        playerInput.CommonGamepad.MinorActionHold.performed += context => DoMinorActionHold();

        playerInput.CommonGamepad.AnyTargetSelection.performed += context => AnyTargetSelection();
        playerInput.CommonGamepad.AnyTargetSelection.canceled += context => AnyTargetSelectionCancel();

        playerInput.PlayerGamepad.FastMove.performed += context => FastMove();
        playerInput.PlayerGamepad.FastMove.canceled += context => FastMoveCancel();

        playerInput.CommonGamepad.Take.performed += context => Take();

        #region Vertical Move

        playerInput.PlayerGamepad.LeftUp.performed += context => SetVerticalMove(VerticalMoveDirection.LeftUp);
        playerInput.PlayerGamepad.RightUp.performed += context => SetVerticalMove(VerticalMoveDirection.RightUp);
        playerInput.PlayerGamepad.LeftDown.performed += context => SetVerticalMove(VerticalMoveDirection.LeftDown);
        playerInput.PlayerGamepad.RightDown.performed += context => SetVerticalMove(VerticalMoveDirection.RightDown);

        playerInput.PlayerGamepad.LeftUp.canceled += context => CancelVerticalMove(VerticalMoveDirection.LeftUp);
        playerInput.PlayerGamepad.RightUp.canceled += context => CancelVerticalMove(VerticalMoveDirection.RightUp);
        playerInput.PlayerGamepad.LeftDown.canceled += context => CancelVerticalMove(VerticalMoveDirection.LeftDown);
        playerInput.PlayerGamepad.RightDown.canceled += context => CancelVerticalMove(VerticalMoveDirection.RightDown);

        #endregion
    }
}
