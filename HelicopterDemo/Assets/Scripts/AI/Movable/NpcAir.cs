using System.Collections.Generic;
using UnityEngine;
using static Types;

[RequireComponent(typeof(NpcExplorer))]
[RequireComponent(typeof(NpcMoveToTgt))]
[RequireComponent(typeof(NpcAttack))]
[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NpcTakeoff))]
[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]
[RequireComponent(typeof(CargoItem))]

public class NpcAir : Npc
{
    [SerializeField] private float verticalSpeed = 20f;
    [SerializeField] private float minHeight = 15f;
    [SerializeField] private float maxHeight = 50f;
    [SerializeField] private GameObject airDustPrefab;

    private NpcTakeoff npcTakeoff;
    private List<SimpleRotor> rotors;
    private LineRenderer lineToTarget;
    private AirDuster airDuster;
    private Caravan caravan;

    #region Properties

    #region For change state

    private bool EndOfTakeoff => npcTakeoff.EndOfTakeoff; //2
    private bool NpcUnderAttack //17
    {
        get => health.IsUnderAttack;
        set => health.IsUnderAttack = value;
    }

    #endregion

    public float VerticalSpeed => verticalSpeed;
    public float HeightDelta => distDelta;
    public float MinHeight => minHeight;
    public float MaxHeight => maxHeight;
    public override Vector3 NpcPos => transform.position;
    public override Vector3 NpcCurrDir => rotation.CurrentDirection;
    public LineRenderer LineToTarget => lineToTarget;

    #endregion

    private void Awake()
    {
        base.Init();

        npcTakeoff = GetComponent<NpcTakeoff>();
        thisItem = GetComponent<CargoItem>();
        rotors = new List<SimpleRotor>();
        rotors.AddRange(GetComponentsInChildren<SimpleRotor>());

        lineToTarget = gameObject.AddComponent<LineRenderer>();
        lineToTarget.enabled = false;

        npcState = NpcState.Delivery;

        if (airDustPrefab)
            airDuster = Instantiate(airDustPrefab, transform).GetComponent<AirDuster>();

        thisItem.InitCargoItem += InitHelicopter;
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
        EraseLine();
        switch (npcState)
        {
            case NpcState.Takeoff:
                npcTakeoff.Move();
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

            //for caravan
            case NpcState.CatchUpCaravan:
                transform.position = caravan.GetEscortItemTargetPosition(gameObject);
                transform.rotation = caravan.transform.rotation;
                npcState = NpcState.FollowCaravan;
                break;
            case NpcState.FollowCaravan:
                translation.SetHorizontalTranslation(caravan.Speed);
                break;
            case NpcState.DefendCaravan:
                npcAttack.Move();
                npcAttack.Shoot();
                break;
        }
    }

    
    private void ChangeState()
    {
        switch (npcState)
        {
            case NpcState.Delivery:
                if (transform.position.y <= thisItem.CargoPlatform.transform.position.y)
                {
                    transform.position = thisItem.CargoPlatform.transform.position;
                    foreach (var rotor in rotors)
                        rotor.StartRotor();

                    if (airDuster)
                        airDuster.normRotorSpeed = rotors[0].RotSpeedCoef;
                }
                if (rotors[0].ReadyToTakeoff)
                {
                    if (airDuster)
                    {
                        airDuster.normRotorSpeed = 1f;
                        airDuster.normAltitiude = 0f;
                    }
                    npcState = NpcState.Takeoff;
                }
                break;
            case NpcState.Takeoff:
                if (airDuster) airDuster.normAltitiude = npcTakeoff.altCoef;
                if (EndOfTakeoff)
                {
                    if (airDuster) airDuster.normAltitiude = 1f;
                    npcState = caravan ? NpcState.CatchUpCaravan : NpcState.Exploring;
                }
                break;
            case NpcState.Exploring:
                if (EnemyForPursuit)
                    npcState = NpcState.MoveToTarget;
                else if (NpcUnderAttack)
                {
                    npcState = NpcState.MoveToTarget;
                    selectedTarget = health.AttackSource.gameObject;
                    NpcUnderAttack = false;
                }
                break;
            case NpcState.MoveToTarget:
                if (EnemyForAttack)
                    npcState = NpcState.Attack;
                else if (EnemyLost)
                {
                    npcState = NpcState.Exploring;
                }
                break;
            case NpcState.Attack:
                if (EnemyForPursuit)
                    npcState = NpcState.MoveToTarget;
                else if (EnemyLost)
                    npcState = NpcState.Exploring;
                break;

            //for caravan
            case NpcState.CatchUpCaravan:

                break;
            case NpcState.FollowCaravan:
                if (EnemyForAttack)
                    npcState = NpcState.DefendCaravan;
                break;
            case NpcState.DefendCaravan:
                if (EnemyLost)
                    npcState = NpcState.CatchUpCaravan;
                break;
        }
    }

    private void SelectTarget()
    {
        KeyValuePair<GameObject, float> nearestNpc, nearestPlayer;
        if (IsFriendly)
        {
            nearestNpc = npcController.FindNearestEnemy(transform.position);
        }
        else
        {
            nearestNpc = npcController.FindNearestFriendly(transform.position);
            nearestPlayer = npcController.FindNearestPlayer(transform.position);
            nearestNpc = nearestPlayer.Value < nearestNpc.Value ? nearestPlayer : nearestNpc;
        }

        switch (npcState)
        {
            case NpcState.Attack:
                selectedTarget = nearestNpc.Key;
                break;
            case NpcState.MoveToTarget:
                selectedTarget = nearestNpc.Value > MaxPursuitDist ? null : nearestNpc.Key;
                break;
            default:
                selectedTarget = nearestNpc.Value <= MinPursuitDist ? nearestNpc.Key : null;
                break;

        }
    }

    private void DrawLine(Color color)
    {
        LineToTarget.enabled = true;
        LineToTarget.startColor = color;
        LineToTarget.endColor = color;
        LineToTarget.SetPosition(0, transform.position);
        LineToTarget.SetPosition(1, selectedTarget.transform.position);
    }

    private void EraseLine() => LineToTarget.enabled = false;

    private void InitHelicopter(Caravan caravan)
    {
        if (caravan) caravan.AddEscortItem(gameObject);
        this.caravan = caravan;
    }
}
