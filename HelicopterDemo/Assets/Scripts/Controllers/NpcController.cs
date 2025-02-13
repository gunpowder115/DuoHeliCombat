using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    #region Properties

    public static NpcController singleton { get; private set; }
    public List<GameObject> NPCs => npcs;
    public int EnemyAirCount => GetNpcCount(enemyAirTag);
    public int FriendlyAirCount => GetNpcCount(friendlyAirTag);
    public int EnemyGroundCount => GetNpcCount(enemyGroundTag);
    public int FriendlyGroundCount => GetNpcCount(friendlyGroundTag);
    public int EnemyBuildCount => GetNpcCount(enemyBuildTag);
    public int FriendlyBuildCount => GetNpcCount(friendlyBuildTag);

    #endregion

    #region Readonly

    readonly string enemyAirTag = "EnemyAir";
    readonly string friendlyAirTag = "FriendlyAir";
    readonly string enemyGroundTag = "EnemyGround";
    readonly string friendlyGroundTag = "FriendlyGround";
    readonly string enemyBuildTag = "EnemyBuild";
    readonly string friendlyBuildTag = "FriendlyBuild";
    readonly string playerTag = "Player";

    #endregion

    private List<GameObject> players;
    List<GameObject> npcs;

    public void Add(GameObject npc)
    {
        if (!npcs.Contains(npc))
            npcs.Add(npc);
    }

    public void Remove(GameObject npc)
    {
        if (npcs.Contains(npc))
            npcs.Remove(npc);
    }

    public Dictionary<GameObject, float> FindDistToEnemies(in Vector3 origin) => FindDistToNpcs(in origin, true);
    public Dictionary<GameObject, float> FindDistToFriendlies(in Vector3 origin) => FindDistToNpcs(in origin, false);
    public KeyValuePair<GameObject, float> FindNearestEnemy(in Vector3 origin) => FindNearestNpc(in origin, FindDistToEnemies(in origin));
    public KeyValuePair<GameObject, float> FindNearestFriendly(in Vector3 origin) => FindNearestNpc(in origin, FindDistToFriendlies(in origin));
    public KeyValuePair<GameObject, float> FindNearestPlayer(in Vector3 origin)
    {
        bool arePlayers = players.Count > 0;
        KeyValuePair<GameObject, float> nearestPlayer = arePlayers ? FindDistToPlayers(origin).ElementAt(0) : new KeyValuePair<GameObject, float>(null, Mathf.Infinity);
        return nearestPlayer;
    }

    void Awake()
    {
        singleton = this;
        npcs = new List<GameObject>();
        players = new List<GameObject>();
        players.AddRange(GameObject.FindGameObjectsWithTag(playerTag));
    }

    int GetNpcCount(string tag)
    {
        int count = 0;
        foreach (var npc in npcs)
            if (npc.CompareTag(tag)) count++;
        return count;
    }

    Dictionary<GameObject, float> FindDistToNpcs(in Vector3 origin, bool findEnemies = true)
    {
        string selAirTag = findEnemies ? enemyAirTag : friendlyAirTag;
        string selGroundTag = findEnemies ? enemyGroundTag : friendlyGroundTag;
        string selBuildTag = findEnemies ? enemyBuildTag : friendlyBuildTag;
        Dictionary<GameObject, float> result = new Dictionary<GameObject, float>();

        foreach (var npc in npcs)
        {
            if (npc.CompareTag(selAirTag) || npc.CompareTag(selGroundTag) || npc.CompareTag(selBuildTag))
            {
                float distTo = Vector3.Magnitude(npc.transform.position - origin);
                result.Add(npc, distTo);
            }
        }
        var sortedResult = result.OrderBy(npc => npc.Value)
                              .ToDictionary(npc => npc.Key, npc => npc.Value);

        return sortedResult;
    }

    private Dictionary<GameObject, float> FindDistToPlayers(in Vector3 origin)
    {
        Dictionary<GameObject, float> result = new Dictionary<GameObject, float>();

        foreach (var player in players)
        {
            float distTo = Vector3.Magnitude(player.transform.position - origin);
            result.Add(player, distTo);
        }
        var sortedResult = result.OrderBy(npc => npc.Value)
                      .ToDictionary(npc => npc.Key, npc => npc.Value);

        return sortedResult;
    }

    KeyValuePair<GameObject, float> FindNearestNpc(in Vector3 origin, Dictionary<GameObject, float> npcs)
    {
        bool areNpcs = npcs.Count > 0;
        KeyValuePair<GameObject, float> nearestNpc = areNpcs ? npcs.ElementAt(0) : new KeyValuePair<GameObject, float>(null, Mathf.Infinity);
        return nearestNpc;
    }
}
