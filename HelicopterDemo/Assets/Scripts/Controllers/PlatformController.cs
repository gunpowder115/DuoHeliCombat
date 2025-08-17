using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformController
{
    public static PlatformController Singleton
    {
        get
        {
            if (singleton == null)
                singleton = new PlatformController();
            return singleton;
        }
    }

    private List<GameObject> platforms;
    private static PlatformController singleton;

    private PlatformController()
    {
        platforms = new List<GameObject>();
    }

    public void Add(GameObject platform)
    {
        if (!platforms.Contains(platform))
            platforms.Add(platform);
    }

    public void Remove(GameObject platform)
    {
        if (platforms.Contains(platform))
            platforms.Remove(platform);
    }

    public GameObject FindClosesPlatform(GameObject src, out float dist)
    {
        GameObject closestPlatform = null;
        float closestSqrDistance = float.MaxValue;

        foreach (var platform in platforms)
        {
            float sqrDistance = Vector3.SqrMagnitude(src.transform.position - platform.transform.position);
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closestPlatform = platform;
            }
        }

        dist = Vector3.Magnitude(src.transform.position - closestPlatform.transform.position);
        return closestPlatform;
    }
}
