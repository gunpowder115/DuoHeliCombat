using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] buildingPrefabs;

    public void SetBuilding(int buildNumber)
    {
        Instantiate(buildingPrefabs[buildNumber - 1], transform);
    }
}
