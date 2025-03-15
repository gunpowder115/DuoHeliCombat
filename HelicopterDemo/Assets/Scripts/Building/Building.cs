using UnityEngine;
using static Types;

public class Building : MonoBehaviour
{
    [SerializeField] private GlobalSide2 buildingSide = GlobalSide2.Blue;
    [SerializeField] private GameObject deadPrefab;
    [SerializeField] private GameObject explosion;

    private Platform platform;
    private NpcController npcController;
    private PlatformController platformController;

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
        npcController = NpcController.Singleton;
        platformController = PlatformController.Singleton;
        npcController.Add(gameObject);
    }

    public void Remove() => CommandCenter.RemoveBuilding(this);

    public void RequestDestroy()
    {
        npcController.Remove(gameObject);
        platformController.Add(platform.gameObject);

        if (deadPrefab) Instantiate(deadPrefab, transform.position, transform.rotation);
        if (explosion) Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
        if (platform) platform.HidePlatform();

        Remove();
        Destroy(gameObject);
    }
}
