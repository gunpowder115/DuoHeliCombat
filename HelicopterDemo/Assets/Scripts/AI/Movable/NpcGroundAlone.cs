using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NpcAttack))]
[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]

public class NpcGroundAlone : Npc
{
    #region Properties

    public override Vector3 NpcPos => transform.position;
    public override Vector3 NpcCurrDir => rotation.CurrentDirection;

    #endregion

    private void Awake()
    {
        base.Init();

        thisItem = GetComponent<CargoItem>();
        npcState = NpcState.Exploring;
        npcController.Add(gameObject);
    }

    void Update()
    {
        SelectTarget();
        SetTrackersRotation();
        ChangeState();
        Move();
    }

    public void RemoveTarget() => selectedTarget = null;

    public override void RequestDestroy()
    {
        npcController.Remove(gameObject);

        if (deadPrefab)
            Instantiate(deadPrefab, transform.position, transform.rotation);

        if (explosion)
            Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);

        Destroy(gameObject);
    }

    private void Move()
    {
        switch (npcState)
        {
            case NpcState.Attack:
                npcAttack.Shoot();
                break;
        }
    }


    private void ChangeState()
    {
        switch (npcState)
        {
            case NpcState.Exploring:
                if (EnemyForPursuit)
                    npcState = NpcState.MoveToTarget;
                break;
            case NpcState.MoveToTarget:
                if (EnemyForAttack)
                    npcState = NpcState.Attack;
                else if (EnemyLost && IsExplorer)
                    npcState = NpcState.Exploring;
                break;
            case NpcState.Attack:
                if (EnemyForPursuit)
                    npcState = NpcState.MoveToTarget;
                else if (EnemyLost && IsExplorer)
                    npcState = NpcState.Exploring;
                break;
        }
    }

    private void SelectTarget()
    {
        KeyValuePair<float, GameObject> nearestNpc, nearestPlayer;
        if (IsFriendly)
        {
            nearestNpc = npcController.FindNearestEnemy(transform.position);
        }
        else
        {
            nearestNpc = npcController.FindNearestFriendly(transform.position);
            nearestPlayer = npcController.FindNearestPlayer(transform.position);
            nearestNpc = nearestPlayer.Key < nearestNpc.Key ? nearestPlayer : nearestNpc;
        }

        switch (npcState)
        {
            case NpcState.Attack:
                selectedTarget = nearestNpc.Value;
                break;
            case NpcState.MoveToTarget:
                selectedTarget = nearestNpc.Key > MaxPursuitDist ? null : nearestNpc.Value;
                break;
            default:
                selectedTarget = nearestNpc.Key <= MinPursuitDist ? nearestNpc.Value : null;
                break;
        }
    }
}
