using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private List<CommandCenter> commandCenters;

    private void Awake()
    {
        commandCenters = new List<CommandCenter>();
        commandCenters.AddRange(FindObjectsOfType<CommandCenter>());
    }
}
