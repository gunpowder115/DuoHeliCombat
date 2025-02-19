using System.Collections.Generic;
using UnityEngine;
using static Types;

public class CommandCenter : MonoBehaviour
{
    [SerializeField] private float distToPlatform = 20f;
    [SerializeField] private GameObject redAttributesPrefab;
    [SerializeField] private GameObject blueAttributesPrefab;
    [SerializeField] private GameObject neutralAttributePrefab;

    private List<Platform> platforms;
    private List<Building> buildings;
    private GameObject redAttribute, blueAttribute, neutralAttribute;
    private GlobalSide3 commandCenterSide;
    private PlatformController platformController;

    private void Awake()
    {
        platformController = PlatformController.Singleton;
        platforms = new List<Platform>();
        platforms.AddRange(GetComponentsInChildren<Platform>());
        PlaceAllPlatforms();

        commandCenterSide = GlobalSide3.Neutral;
    }

    private void Start()
    {
        InitBuildings();
    }

    private void Update()
    {
        if (buildings.Count == 0)
        {
            commandCenterSide = GlobalSide3.Neutral;
        }

        switch (commandCenterSide)
        {
            case GlobalSide3.Neutral:
                if (redAttribute) Destroy(redAttribute);
                if (blueAttribute) Destroy(blueAttribute);
                if (neutralAttributePrefab && !neutralAttribute) neutralAttribute = Instantiate(neutralAttributePrefab, transform);
                break;
            case GlobalSide3.Blue:
                if (redAttribute) Destroy(redAttribute);
                if (neutralAttribute) Destroy(neutralAttribute);
                if (blueAttributesPrefab && !blueAttribute) blueAttribute = Instantiate(blueAttributesPrefab, transform);
                break;
            case GlobalSide3.Red:
                if (blueAttribute) Destroy(blueAttribute);
                if (neutralAttribute) Destroy(neutralAttribute);
                if (redAttributesPrefab && !redAttribute) redAttribute = Instantiate(redAttributesPrefab, transform);
                break;
        }
    }

    public void RemoveBuilding(Building building)
    {
        if (buildings.Contains(building)) buildings.Remove(building);
    }

    private void PlaceAllPlatforms()
    {
        if (platforms.Count != BUILDING_COUNT)
            Debug.LogError(this.ToString() + ": platformComponents.Length = " + platforms.Count);

        for (int i = 0; i < platforms.Count; i++)
        {
            Vector3 toPlatform = (platforms[i].transform.position - transform.position).normalized * distToPlatform;
            Vector3 platfromTranslation = (transform.position + toPlatform) - platforms[i].transform.position;
            platforms[i].transform.Translate(platfromTranslation, Space.World);
            platforms[i].HidePlatform();

            platforms[i].SetCommandCenter(this);
            platformController.Add(platforms[i].gameObject);
        }
    }

    private void InitBuildings()
    {
        buildings = new List<Building>();
        foreach (var platform in platforms)
        {
            if (platform.Building)
            {
                platform.ShowPlatform();
                buildings.Add(platform.Building);
                if (commandCenterSide == GlobalSide3.Neutral) commandCenterSide = platform.GetPlatformSide();
                platformController.Remove(platform.gameObject);
            }
        }
    }
}
