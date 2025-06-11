using Assets.Scripts.Controllers;
using System;
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

    public List<IFindable> Players => players;

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

    public T FindClosestEnemy<T>(IFindable src, out float dist) where T : class, IFindable
    {
        List<T> possibleTargets = new List<T>();

        if (typeof(T) == typeof(Npc))
        {
            foreach (var npc in npcs)
            {
                if (npc.Side != src.Side)
                    possibleTargets.Add(npc as T);
            }
        }
        else if (typeof(T) == typeof(Building))
        {
            foreach (var building in buildings)
            {
                if (building.Side != src.Side)
                    possibleTargets.Add(building as T);
            }
        }
        else if (typeof(T) == typeof(Player))
        {
            foreach (var player in players)
            {
                if (player.Side != src.Side)
                    possibleTargets.Add(player as T);
            }
        }
        else
        {
            throw new ArgumentException($"Unsupported type: {typeof(T)}");
        }

        if (possibleTargets.Count == 0)
        {
            dist = float.MaxValue;
            return null;
        }

        T closestEnemy = null;
        float closestSqrDistance = float.MaxValue;

        foreach (var target in possibleTargets)
        {
            float sqrDistance = Vector3.SqrMagnitude(src.Position - target.Position);
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closestEnemy = target;
            }
        }

        dist = Vector3.Magnitude(src.Position - closestEnemy.Position); ;
        return closestEnemy;
    }

    public IFindable FindClosestEnemy(IFindable src, out float dist)
    {
        IFindable closestEnemy = null;
        float closestSqrDistance = float.MaxValue;

        foreach (var npc in npcs)
        {
            if (npc.Side != src.Side)
            {
                float sqrDistance = Vector3.SqrMagnitude(src.Position - npc.Position);
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestEnemy = npc;
                }
            }
        }

        foreach (var building in buildings)
        {
            if (building.Side != src.Side)
            {
                float sqrDistance = Vector3.SqrMagnitude(src.Position - building.Position);
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestEnemy = building;
                }
            }
        }

        foreach (var player in players)
        {
            if (player.Side != src.Side)
            {
                float sqrDistance = Vector3.SqrMagnitude(src.Position - player.Position);
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestEnemy = player;
                }
            }
        }

        dist = Vector3.Magnitude(src.Position - closestEnemy.Position);
        return closestEnemy;
    }

    public Player FindClosestPlayer(IFindable src, out float dist)
    {
        if (players.Count == 0)
        {
            dist = float.MaxValue;
            return null;
        }

        IFindable closestPlayer = null;
        float closestSqrDistance = float.MaxValue;

        foreach (var player in players)
        {
            Vector3 offset = player.Position - src.Position;
            float sqrDistance = offset.sqrMagnitude;

            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closestPlayer = player;
            }
        }

        dist = Vector3.Distance(closestPlayer.Position, src.Position);
        return closestPlayer as Player;
    }

    public Player FindClosestPlayer(IFindable src)
    {
        float dist;
        return FindClosestPlayer(src, out dist);
    }

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
}
