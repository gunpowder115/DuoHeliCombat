using UnityEngine;
using static Types;

public class Platform : MonoBehaviour
{
    private bool isActive;

    public bool isFree => !Building.gameObject;
    public CommandCenter CommandCenter { get; private set; }
    public Building Building { get; private set; }

    private void Update()
    {
        if (Building && !isActive)
        {
            transform.Translate(0f, 0f, 0f);
            isActive = true;
        }
        else if (!Building && isActive)
        {
            transform.Translate(0f, -1f, 0f);
            isActive = false;
        }
    }

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
