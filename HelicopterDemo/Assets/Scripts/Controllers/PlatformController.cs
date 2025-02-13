using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public static PlatformController singleton {get; private set;}
    public List<GameObject> Platforms => platforms;

    readonly string platformTag = "Platform";

    List<GameObject> platforms;

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

    void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        platforms = new List<GameObject>();
        platforms.AddRange(GameObject.FindGameObjectsWithTag(platformTag));
    }
}
