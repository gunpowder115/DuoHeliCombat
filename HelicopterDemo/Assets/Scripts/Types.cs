public class Types
{
    #region Constants

    public const int BUILDING_COUNT = 8;

    #endregion

    #region Enumerations

    public enum GlobalSide3
    {
        Neutral,
        Red,
        Blue
    }

    public enum GlobalSide2
    {
        Red,
        Blue
    }

    public enum NpcState
    {
        //default
        Idle,
        Delivery,
        Takeoff,
        Exploring,
        MoveToTarget,
        Attack,

        //for caravan
        CatchUpCaravan,
        FollowCaravan,
        DefendCaravan,
    }

    public enum Axes
    {
        X, Y, Z,
        X_Y_Z
    }

    public enum CargoType
    {
        Delivering,
        Dropping
    }

    public enum WingState
    {
        None,
        Short,
        Long
    }

    public enum WeaponType
    {
        None,
        Minigun,
        UnguidMissile,
        GuidMissile
    }

    public struct WingWeaponConfig
    {
        public WingState wingStateLeft;
        public WeaponType L2;
        public WeaponType L1;
        public WeaponType C;
        public WingState wingStateRight;
        public WeaponType R1;
        public WeaponType R2;
    }

    public enum SlotType
    {
        L2, L1, C, R1, R2
    }

    public enum KeyType : int
    {
        Red,
        Blue,
        Green,
        Yellow
    }

    public enum FadingScreenType
    {
        None,
        Darkening,
        FullDark,
        Lightening
    }

    public enum PickableUpType
    {
        BlueKey,
        GreenKey,
        RedKey,
        YellowKey,
        BlueFlag,
        RedFlag,
        Bomb,
        Ladder
    }

    #endregion
}
