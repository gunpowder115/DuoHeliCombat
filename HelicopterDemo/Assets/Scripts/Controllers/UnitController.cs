using Assets.Scripts.Controllers;
using System.Collections.Generic;
using UnityEngine;

public class UnitController
{
    #region Properties

    public static UnitController Singleton
    {
        get
        {
            if (singleton == null)
                singleton = new UnitController();
            return singleton;
        }
    }

    #endregion

    private List<IFindable> players;
    private List<IFindable> npcs;
    private List<IFindable> buildings;
    private static UnitController singleton;

    private UnitController()
    {
        npcs = new List<IFindable>();
        buildings = new List<IFindable>();
        players = new List<IFindable>();
    }

    public void AddPlayer(Player player) => Add(players, player);
    public void RemovePlayer(Player player) => Remove(players, player);
    public void AddNpc(Npc npc) => Add(npcs, npc);
    public void RemoveNpc(Npc npc) => Remove(npcs, npc);
    public void AddBuilding(Building building) => Add(buildings, building);
    public void RemoveBuilding(Building building) => Remove(buildings, building);

    public Npc FindNearestEnemyNpcForMe(IFindable me, out float dist) => FindNearestEntityForMe(npcs, me, out dist) as Npc;
    public Building FindNearestEnemyBuildingForMe(IFindable me, out float dist) => FindNearestEntityForMe(buildings, me, out dist) as Building;
    public Player FindNearestEnemyPlayerForMe(IFindable me, out float dist) => FindNearestEntityForMe(players, me, out dist) as Player;
    public Player FindNearestPlayerForMe(IFindable me, out float dist) => FindNearestEntityForMe(players, me, out dist, false) as Player;

    private void Add(List<IFindable> list, IFindable item)
    {
        if (!list.Contains(item))
            list.Add(item);
    }

    private void Remove(List<IFindable> list, IFindable item)
    {
        if (list.Contains(item))
            list.Remove(item);
    }

    private T FindNearestEntityForMe<T>(List<T> units, IFindable me, out float dist, bool enemyOnly = true) where T : class, IFindable
    {
        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        IFindable result = null;
        if (units != null && units.Count > 0)
        {
            foreach (var unit in units)
            {
                if (enemyOnly && unit.Side == me.Side) continue;
                float x = Mathf.Abs(unit.Position.x - me.Position.x);
                float y = Mathf.Abs(unit.Position.y - me.Position.y);
                if (x < minX || y < minY)
                {
                    minX = x;
                    minY = y;
                    result = unit;
                }
            }
        }
        dist = Vector3.Distance(result.Position, me.Position);
        return result as T;
    }
}
