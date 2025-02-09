using UnityEngine;

public class Building : MonoBehaviour
{
    public CommandCenter CommandCenter => Platform.CommandCenter;
    public Platform Platform { get; private set; }

    public void SetPlatform(Platform platform) => Platform = platform;
}
