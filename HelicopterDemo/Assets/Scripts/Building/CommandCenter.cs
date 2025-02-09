using UnityEngine;
using static Types;

[RequireComponent(typeof(InitialBuilder))]

public class CommandCenter : MonoBehaviour
{
    [SerializeField] private CommandCenterSide commandCenterSide = CommandCenterSide.Neutral;

    private InitialBuilder initialBuilder;

    public Platform[] Platforms { get; private set; }
    public GameObject[] Buildings { get; private set; }
    public CargoItem[] CargoItems { get; private set; }

    private void Awake()
    {
        initialBuilder = GetComponent<InitialBuilder>();
        Platforms = initialBuilder.GetPlatforms(this);
        Buildings = initialBuilder.InitBuildings(Platforms);
    }
}
