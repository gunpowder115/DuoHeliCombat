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

    public Dictionary<GameObject, float> FindDistToPlatforms(in Vector3 origin)
    {
        Dictionary<GameObject, float> result = new Dictionary<GameObject, float>();

        foreach (var platform in platforms)
        {
            if (!result.ContainsKey(platform))
            {
                float distTo = Vector3.Magnitude(platform.transform.position - origin);
                result.Add(platform, distTo);
            }
        }
        return result;
    }

    public KeyValuePair<GameObject, float> FindNearestPlatform(in Vector3 origin)
    {
        Dictionary<GameObject, float> platforms = FindDistToPlatforms(in origin);
        bool arePlatforms = platforms.Count > 0;
        KeyValuePair<GameObject, float> nearestPlatform = arePlatforms ? platforms.ElementAt(0) : new KeyValuePair<GameObject, float>(null, Mathf.Infinity);
        return nearestPlatform;
    }
}
