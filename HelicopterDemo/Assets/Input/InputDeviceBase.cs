using System;
using UnityEngine;
using static Types;

public abstract class InputDeviceBase
{
    #region Events

    public event Func<PlayerStates, PlayerStates> TryBindingToObject;
    public event Action TryLaunchUnguidedMissile;
    public event Action CancelBuildSelection;
    public event Action TryLaunchGuidedMissile;
    public event Action StartSelectionFarTarget;
    public event Action StartSelectionAnyTarget;
    public event Action CancelSelectionAnytarget;
    public event Action CancelAiming;

    public event Action<int> ChangePlayerConnection;
    public event Action ChangeConfiguration;
    public event Action ChangeOrientation;

    public event Action<int, GlobalSide2> SelectBuildingEvent;

    public event Action<int> WeaponUpEvent, WeaponDownEvent;
    public event Action SlotLeftEvent, SlotRightEvent;
    public event Action WingUpEvent, WingDownEvent;

    #endregion

    #region Properties

    public InputDeviceBase controllerItem { get; protected set; }
    public bool MinigunFire { get; private set; }
    public bool FastMoving { get; private set; }
    public bool UnguidedMissileLaunch
    {
        get
        {
            if (unguidedMissileLaunch) { unguidedMissileLaunch = false; return true; }
            else return false;
        }
        private set { }
    }
    public bool GuidedMissileLaunch
    {
        get
        {
            if (guidedMissileLaunch) { guidedMissileLaunch = false; return true; }
            else return false;
        }
        private set { }
    }
    public bool PlayerCanTranslate => playerState == PlayerStates.Normal || playerState == PlayerStates.Aiming;
    public bool AimMovement => playerState == PlayerStates.SelectionAnyTarget || playerState == PlayerStates.SelectionFarTarget;
    public float VerticalMoving
    {
        get
        {
            bool up = ((LeftUp || RightUp) && LeftUp != RightUp) || SingleUp;
            bool down = ((LeftDown || RightDown) && LeftDown != RightDown) || SingleDown;

            if (up) return 1f;
            if (down) return -1f;
            return 0f;
        }
    }
    public float VerticalFastMoving
    {
        get
        {
            if (fastUp) return 1f;
            if (fastDown) return -1f;
            return 0f;
        }
    }
    public PlayerStates PlayerState => playerState;

    #region Private

    bool LeftUp
    {
        get => verticalInputButtons[leftUpIndex];
        set => verticalInputButtons[leftUpIndex] = value;
    }
    bool RightUp
    {
        get => verticalInputButtons[rightUpIndex];
        set => verticalInputButtons[rightUpIndex] = value;
    }
    bool SingleUp
    {
        get => verticalInputButtons[singleUpIndex];
        set => verticalInputButtons[singleUpIndex] = value;
    }
    bool LeftDown
    {
        get => verticalInputButtons[leftDownIndex];
        set => verticalInputButtons[leftDownIndex] = value;
    }
    bool RightDown
    {
        get => verticalInputButtons[rightDownIndex];
        set => verticalInputButtons[rightDownIndex] = value;
    }
    bool SingleDown
    {
        get => verticalInputButtons[singleDownIndex];
        set => verticalInputButtons[singleDownIndex] = value;
    }

    #endregion

    #endregion

    readonly int verticalInputButtonsCount = 6;
    readonly int leftUpIndex = (int)VerticalMoveDirection.LeftUp;
    readonly int rightUpIndex = (int)VerticalMoveDirection.RightUp;
    readonly int singleUpIndex = (int)VerticalMoveDirection.SingleUp;
    readonly int leftDownIndex = (int)VerticalMoveDirection.LeftDown;
    readonly int rightDownIndex = (int)VerticalMoveDirection.RightDown;
    readonly int singleDownIndex = (int)VerticalMoveDirection.SingleDown;

    bool unguidedMissileLaunch, guidedMissileLaunch;
    bool fastUp, fastDown;
    bool[] verticalInputButtons;
    PlayerStates playerState;
    protected PlayerInput playerInput;

    public abstract Vector2 GetInput();

    public abstract Vector2 GetCameraInput();

    public void ForceChangePlayerState(PlayerStates newState) => playerState = newState;

    protected InputDeviceBase() { }

    protected void Init()
    {
        playerInput = new PlayerInput();
        verticalInputButtons = new bool[verticalInputButtonsCount];
    }

    protected void DoMainAction()
    {
        if (playerState == PlayerStates.Normal || playerState == PlayerStates.Aiming)
            MinigunFire = true;
    }

    protected void DoMainActionCancel() => MinigunFire = false;

    protected void DoMinorAction()
    {
        switch (playerState)
        {
            case PlayerStates.Normal:
                playerState = (TryBindingToObject?.Invoke(playerState)).Value;
                break;
            case PlayerStates.Aiming:
                TryLaunchUnguidedMissile?.Invoke();
                break;
            case PlayerStates.BuildSelection:
                CancelBuildSelection?.Invoke();
                playerState = PlayerStates.Normal;
                break;
            case PlayerStates.SelectionFarTarget:
                TryLaunchGuidedMissile?.Invoke();
                playerState = PlayerStates.Normal;
                break;
        }
    }

    protected void DoMinorActionHold()
    {
        switch (playerState)
        {
            case PlayerStates.Normal:
                StartSelectionFarTarget?.Invoke();
                playerState = PlayerStates.SelectionFarTarget;
                break;
        }
    }

    protected void AnyTargetSelection()
    {
        switch (playerState)
        {
            case PlayerStates.Normal:
                StartSelectionAnyTarget?.Invoke();
                playerState = PlayerStates.SelectionAnyTarget;
                break;
        }
    }

    protected void AnyTargetSelectionCancel()
    {
        switch (playerState)
        {
            case PlayerStates.SelectionAnyTarget:
                CancelSelectionAnytarget?.Invoke();
                playerState = PlayerStates.Normal;
                break;
        }
    }

    protected void FastMove()
    {
        switch (playerState)
        {
            case PlayerStates.Normal:
                FastMoving = true;
                break;
            case PlayerStates.Aiming:
                CancelAiming?.Invoke();
                playerState = PlayerStates.Normal;
                break;
        }
    }

    protected void FastMoveCancel() => FastMoving = false;

    protected void SetVerticalMove(VerticalMoveDirection vertMoveDir)
    {
        switch (playerState)
        {
            case PlayerStates.Normal:
                verticalInputButtons[(int)vertMoveDir] = true;
                if (LeftUp && RightUp)
                    fastUp = true;
                else if (LeftDown && RightDown)
                    fastDown = true;
                break;
            case PlayerStates.Aiming:
                verticalInputButtons[(int)vertMoveDir] = true;
                break;
        }
        //no movement if Up & Down pressed in one time
        if (((LeftUp || RightUp) && (LeftDown || RightDown)) || (SingleUp && SingleDown))
        {
            LeftUp = RightUp = LeftDown = RightDown = SingleUp = SingleDown = false;
            fastUp = fastDown = false;
        }
    }

    protected void CancelVerticalMove(VerticalMoveDirection vertMoveDir)
    {
        fastUp = fastDown = false;
        verticalInputButtons[(int)vertMoveDir] = false;
    }

    protected void SetVerticalSingleFast()
    {
        if (SingleUp && !SingleDown) fastUp = true;
        if (SingleDown && !SingleUp) fastDown = true;
    }

    protected void CancelVerticalSingleFast() => fastUp = fastDown = false;

    protected void SwitchPlayerConnection(int playerNumber) => ChangePlayerConnection?.Invoke(playerNumber);

    protected void SwitchConfiguration() => ChangeConfiguration?.Invoke();

    protected void SwitchOrientation() => ChangeOrientation?.Invoke();

    protected void SelectBuilding(int buildNumber, GlobalSide2 side)
    {
        switch(playerState)
        {
            case PlayerStates.BuildSelection:
                SelectBuildingEvent?.Invoke(buildNumber, side);
                ForceChangePlayerState(PlayerStates.Normal);
                break;
        }
    }

    protected void WeaponUp() => WeaponUpEvent?.Invoke(1);
    protected void WeaponDown() => WeaponDownEvent?.Invoke(-1);
    protected void SlotLeft() => SlotLeftEvent?.Invoke();
    protected void SlotRight() => SlotRightEvent?.Invoke();
    protected void WingUp() => WingUpEvent?.Invoke();
    protected void WingDown() => WingDownEvent?.Invoke();

    protected enum VerticalMoveDirection : int
    {
        LeftUp,
        RightUp,
        SingleUp,
        LeftDown,
        RightDown,
        SingleDown
    }

    public enum ControllerType
    {
        Keyboard,
        Gamepad
    }

    public enum PlayerStates
    {
        Normal,
        Aiming,
        SelectionFarTarget,
        SelectionAnyTarget,
        BuildSelection
    }
}
