using Assets.Scripts.Controllers;
using UnityEngine;
using static Types;

public class Building : MonoBehaviour, IFindable
{
    [SerializeField] private GlobalSide2 buildingSide = GlobalSide2.Blue;
    [SerializeField] private GameObject deadPrefab;
    [SerializeField] private GameObject explosion;

    private Platform platform;
    private UnitController unitController;
    private PlatformController platformController;

    public Vector3 Position => transform.position;
    public GlobalSide2 Side => buildingSide;
    public CommandCenter CommandCenter => platform.CommandCenter;
    public GlobalSide2 BuildingSide => buildingSide;

    private void Awake()
    {
        platform = transform.parent.gameObject.GetComponent<Platform>();
        if (platform)
            platform.SetBuilding(this);
    }

    private void Start()
    {
        unitController = UnitController.Singleton;
        platformController = PlatformController.Singleton;
        unitController.AddBuilding(this);
    }

    public void Remove() => CommandCenter.RemoveBuilding(this);

    public void RequestDestroy()
    {
        unitController.RemoveBuilding(this);
        platformController.Add(platform.gameObject);

        if (deadPrefab) Instantiate(deadPrefab, transform.position, transform.rotation);
        if (explosion) Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
        if (platform) platform.HidePlatform();

        Remove();
        Destroy(gameObject);
    }
}
