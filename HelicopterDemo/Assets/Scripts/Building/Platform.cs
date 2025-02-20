using UnityEngine;
using static Types;

public class Platform : MonoBehaviour
{
    private bool isActive;
    private PlatformController platformController;

    public bool IsFree => !Building;
    public CommandCenter CommandCenter { get; private set; }
    public Building Building { get; private set; }

    private void Awake()
    {
        isActive = true;
        platformController = PlatformController.Singleton;
    }

    public void SetCommandCenter(CommandCenter baseCenter) => CommandCenter = baseCenter;

    public void SetBuilding(Building building) => Building = building;

    public void ShowPlatform()
    {
        if (!isActive)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            isActive = true;
        }
    }

    public void HidePlatform()
    {
        if (isActive)
        {
            transform.position = new Vector3(transform.position.x, -1f, transform.position.z);
            isActive = false;
        }
    }

    public GlobalSide3 GetPlatformSide()
    {
        if (!Building.gameObject)
            return GlobalSide3.Neutral;
        else if (Building.BuildingSide == GlobalSide2.Red)
            return GlobalSide3.Red;
        else
            return GlobalSide3.Blue;
    }

    public void InitBuilding(GameObject buildPrefab)
    {
        var build = Instantiate(buildPrefab, transform).GetComponent<Building>();
        SetBuilding(build);
        ShowPlatform();
        CommandCenter.AddBuilding(build);
        if (CommandCenter.CommandCenterSide == GlobalSide3.Neutral) CommandCenter.CommandCenterSide = GetPlatformSide();
        platformController.Remove(gameObject);
    }
}
