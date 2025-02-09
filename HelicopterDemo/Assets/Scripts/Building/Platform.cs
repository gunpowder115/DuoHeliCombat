using UnityEngine;
using static Types;

public class Platform : MonoBehaviour
{
    public bool isFree => !Building.gameObject;
    public CommandCenter CommandCenter { get; private set; }
    public Building Building { get; private set; }

    public void SetCommandCenter(CommandCenter baseCenter) => CommandCenter = baseCenter;
    public void SetBuilding(Building building) => Building = building;
    public GlobalSide3 GetPlatformSide()
    {
        if (!Building.gameObject)
            return GlobalSide3.Neutral;
        else if (Building.BuildingSide == GlobalSide2.Red)
            return GlobalSide3.Red;
        else
            return GlobalSide3.Blue;        
    }
}
