using UnityEngine;
using static Types;

public class BuildingSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] redBuildingsPrefabs;
    [SerializeField] private GameObject[] blueBuildingsPrefabs;

    public void SetBuilding(int buildNumber, GlobalSide2 side)
    {
        GetComponent<Platform>().InitBuilding(side == GlobalSide2.Blue ? blueBuildingsPrefabs[buildNumber - 1] : redBuildingsPrefabs[buildNumber - 1]);
    }
}
