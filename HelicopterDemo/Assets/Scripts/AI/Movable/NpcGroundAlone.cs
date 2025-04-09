using System.Collections.Generic;
using UnityEngine;
using static Types;

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

    private void Start()
    {
        base.Init();

        npcState = NpcState.Exploring;
        unitController.AddNpc(this);
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
        unitController.RemoveNpc(this);

        if (deadPrefab)
            Instantiate(deadPrefab, transform.position, transform.rotation);

        if (explosion)
            Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);

        var building = GetComponent<Building>();
        if (building)
            building.Remove();

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
                else if (EnemyLost)
                    npcState = NpcState.Exploring;
                break;
            case NpcState.Attack:
                if (EnemyForPursuit)
                    npcState = NpcState.MoveToTarget;
                else if (EnemyLost)
                    npcState = NpcState.Exploring;
                break;
        }
    }

    private void SelectTarget()
    {
        float distToNpc = Mathf.Infinity, distToPlayer = Mathf.Infinity;
        var enemyNpc = unitController.FindNearestEnemyNpcForMe(this, out distToNpc);
        var enemyPlayer = unitController.FindNearestEnemyPlayerForMe(this, out distToPlayer);
        GameObject enemy = distToPlayer < distToNpc ? enemyPlayer.gameObject : enemyNpc.gameObject;
        float distToEnemy = Mathf.Min(distToPlayer, distToNpc);

        switch (npcState)
        {
            case NpcState.Attack:
                selectedTarget = enemy;
                break;
            case NpcState.MoveToTarget:
                selectedTarget = distToEnemy > MaxPursuitDist ? null : enemy;
                break;
            default:
                selectedTarget = distToEnemy <= MinPursuitDist ? enemy : null;
                break;
        }
    }
}
