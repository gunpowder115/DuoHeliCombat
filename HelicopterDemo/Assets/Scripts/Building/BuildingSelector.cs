using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] redBuildingsPrefabs;
    [SerializeField] private GameObject[] blueBuildingsPrefabs;

    public void SetBuilding(int buildNumber)
    {
        GetComponent<Platform>().InitBuilding(blueBuildingsPrefabs[buildNumber - 1]);
    }
}
