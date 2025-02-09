using UnityEngine;
using static Types;

public class Building : MonoBehaviour
{
    [SerializeField] private GlobalSide2 buildingSide = GlobalSide2.Blue;

    private Platform platform;

    public CommandCenter CommandCenter => platform.CommandCenter;
    public GlobalSide2 BuildingSide => buildingSide;

    private void Awake()
    {
        platform = transform.parent.gameObject.GetComponent<Platform>();
        if (platform)
            platform.SetBuilding(this);
    }

    public void Remove() => CommandCenter.RemoveBuilding(this);
}
