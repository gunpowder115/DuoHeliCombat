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

    public enum SlotType
    {
        L2, L1, C, R1, R2
    }

    public enum KeyType : int
    {
        Red,
        Blue,
        Purple,
        Yellow
    }

    public enum FadingScreenType
    {
        None,
        Darkening,
        FullDark,
        Lightening
    }

    #endregion
}
