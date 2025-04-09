using System.Collections.Generic;
using UnityEngine;
using static Types;

[RequireComponent(typeof(NpcExplorer))]
[RequireComponent(typeof(NpcMoveToTgt))]
[RequireComponent(typeof(NpcAttack))]

public class NpcSquad : Npc
{
    [SerializeField] private float memberRadius = 3f;

    private Vector3 squadPos;
    private Npc attackSource;
    private Squad squad;

    #region Properties

    private bool IsDead //15
    {
        get
        {
            foreach (var npc in Npcs)
                if (npc.gameObject) return false;
            return true;
        }
    }
    private NpcGround MemberUnderAttack //17
    {
        get
        {
            foreach (var npc in Npcs)
                if (npc.UnderAttack)
                {
                    attackSource = npc.AttackSource;
                    return npc;
                }
            return null;
        }
        set
        {
            foreach (var npc in Npcs)
                if (npc.UnderAttack) npc.UnderAttack = value;
        }
    }

    private Vector3 CurrentDirection => Npcs[0].Rotation.CurrentDirection;
    public override Vector3 NpcPos => squadPos;
    public override Vector3 NpcCurrDir => CurrentDirection;
    public List<NpcGround> Npcs => squad.Npcs;

    #endregion

    private void Awake()
    {
        npcState = NpcState.Delivery;
        squad = GetComponent<Squad>();
        base.Init();
    }

    private void Update()
    {
        if (Npcs.Count > 0)
        {
            SelectTarget();
            SetMembersTrackers();
            ChangeState();
            Move();
        }
    }

    public void MoveSquad(Vector3 targetDir, float speed)
    {
        if (targetDir == Vector3.zero) 
            targetDir = CurrentDirection;

        if (Npcs.Count > 1)
        {
            Vector3[] newNpcSpeed;
            if (Npcs.Count == 2)
                newNpcSpeed = CorrectSpeed_2(targetDir, speed);
            else
                newNpcSpeed = CorrectSpeed_3(targetDir, speed);

            for (int i = 0; i < Npcs.Count; i++)
            {
                Npcs[i].Translate(Npcs[i].NpcCurrDir * speed);
                Npcs[i].Rotation.RotateByYaw(targetDir);

                //check this for use corrected speed!
                //Npcs[i].Translate(Npcs[i].NpcCurrDir * newNpcSpeed[i].magnitude);
                //Npcs[i].Rotation.RotateByYaw(newNpcSpeed[i]);
            }
        }
        else
        {
            squadPos = squad.GetSquadPos(0);
            Npcs[0].Translate(Npcs[0].NpcCurrDir * speed);
            Npcs[0].Rotation.RotateByYaw(targetDir);
        }
    }

    public override void RequestDestroy() => Destroy(gameObject);

    private void SelectTarget()
    {
        float distToNpc = Mathf.Infinity, distToPlayer = Mathf.Infinity;
        var enemyNpc = unitController.FindNearestEnemyNpcForMe(Npcs[0], out distToNpc);
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
            case NpcState.Delivery:
                selectedTarget = null;
                break;
            default:
                selectedTarget = distToEnemy <= MinPursuitDist ? enemy : null;
                break;

        }

        foreach (var npc in Npcs)
            npc.SetTarget(selectedTarget);
    }

    private void ChangeState()
    {
        switch (npcState)
        {
            case NpcState.Delivery:
                if (Npcs[0].gameObject.transform.position.y <= transform.position.y)
                {
                    npcState = NpcState.Exploring;
                    for (int i = 0; i < Npcs.Count; i++)
                    {
                        Npcs[i].Drop(0f);
                        Npcs[i].transform.position = new Vector3(Npcs[i].transform.position.x, transform.position.y, Npcs[i].transform.position.z);
                    }
                }
                break;
            case NpcState.Exploring:
                if (EnemyForPursuit)
                    npcState = NpcState.MoveToTarget;
                else if (MemberUnderAttack != null)
                {
                    npcState = NpcState.MoveToTarget;
                    selectedTarget = attackSource.gameObject;
                    MemberUnderAttack = null;
                }
                break;
            case NpcState.MoveToTarget:
                if (EnemyForAttack)
                    npcState = NpcState.Attack;
                else if (EnemyLost)
                    npcState = NpcState.Exploring;
                break;
            case NpcState.Attack:
                if (EnemyForPursuit)
                {
                    npcState = NpcState.MoveToTarget;
                }
                else if (EnemyLost)
                {
                    npcState = NpcState.Exploring;
                }
                break;
        }
    }

    private void Move()
    {
        switch (npcState)
        {
            case NpcState.Delivery:
                break;
            case NpcState.Exploring:
                npcExplorer.Move();
                break;
            case NpcState.MoveToTarget:
                npcMoveToTgt.Move();
                break;
            case NpcState.Attack:
                npcAttack.Move();
                npcAttack.Shoot();
                break;
        }
    }

    private bool BehindOfSquad(NpcGround member)
    {
        float dot = Vector3.Dot(squadPos - member.gameObject.transform.position, member.CurrentSpeed);
        return dot > 0f;
    }

    private void SetMembersTrackers()
    {
        foreach (var npc in Npcs)
            npc.SetTrackersRotation();
    }

    private Vector3[] CorrectSpeed_3(Vector3 targetDir, float speed)
    {
        Vector3 targetSpeed = targetDir * speed;
        Vector3[] correctedSpeed = new Vector3[] { targetSpeed, targetSpeed, targetSpeed };

        var npc0 = Npcs[0];
        var npc1 = Npcs[1];
        var npc2 = Npcs[2];

        bool[] npcFar = new bool[3];
        npcFar[0] = npc0.FarFrom(npc0, squad.SquadRadius * 1.1f);
        npcFar[1] = npc0.FarFrom(npc1, squad.SquadRadius * 1.1f);
        npcFar[2] = npc0.FarFrom(npc2, squad.SquadRadius * 1.1f);
        bool allNpcFar = npcFar[1] && npcFar[2];

        if (allNpcFar)
            squadPos = squad.GetSquadPos(0);
        else if (npcFar[1] || npcFar[2])
            squadPos = squad.GetSquadPos(0, npcFar[1] ? 2 : 1);
        else
            squadPos = squad.GetSquadPos();

        for (int i = 0; i < Npcs.Count; i++)
        {
            Vector3 newTargetSpeed = targetSpeed;
            if (allNpcFar) //two members are lagging behind first member
            {
                if (i == 0)
                    newTargetSpeed = BehindOfSquad(Npcs[0]) ? newTargetSpeed * highSpeedCoef : newTargetSpeed / highSpeedCoef;
                else if (BehindOfSquad(Npcs[i]))
                    newTargetSpeed = highSpeedCoef * speed * (squadPos - Npcs[i].gameObject.transform.position).normalized;
                else
                    newTargetSpeed /= highSpeedCoef;
            }
            else if (npcFar[i]) //one members is lagging behind first member
            {
                if (BehindOfSquad(Npcs[i]))
                    newTargetSpeed = highSpeedCoef * speed * (squadPos - Npcs[i].gameObject.transform.position).normalized;
                else
                    newTargetSpeed /= highSpeedCoef;
            }

            correctedSpeed[i] = newTargetSpeed;
        }
        return correctedSpeed;
    }

    private Vector3[] CorrectSpeed_2(Vector3 targetDir, float speed)
    {
        Vector3 targetSpeed = targetDir * speed;
        Vector3[] correctedSpeed = new Vector3[] { targetSpeed, targetSpeed };

        var npc0 = Npcs[0];
        var npc1 = Npcs[1];
        bool npcFar = npc0.FarFrom(npc1, squad.SquadRadius);

        if (npcFar)
        {
            squadPos = squad.GetSquadPos(0);
            Vector3 newTargetSpeed = targetSpeed;

            newTargetSpeed = BehindOfSquad(npc0) ? newTargetSpeed * highSpeedCoef : newTargetSpeed / highSpeedCoef;
            correctedSpeed[0] = newTargetSpeed;

            newTargetSpeed = targetSpeed;
            if (BehindOfSquad(npc1))
                newTargetSpeed = highSpeedCoef * speed * (squadPos - npc1.gameObject.transform.position).normalized;
            else
                newTargetSpeed /= highSpeedCoef;
            correctedSpeed[1] = newTargetSpeed;
        }
        else
        {
            squadPos = squad.GetSquadPos(0, 1);
            correctedSpeed[0] = targetSpeed;
            correctedSpeed[1] = targetSpeed;
        }

        return correctedSpeed;
    }
}
