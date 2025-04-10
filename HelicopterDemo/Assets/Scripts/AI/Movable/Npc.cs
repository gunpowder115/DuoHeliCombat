using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Types;

public abstract class Npc : MonoBehaviour, IFindable
{
    [SerializeField] protected GlobalSide2 npcSide = GlobalSide2.Blue;
    [SerializeField] protected bool isFriendly = true;
    [SerializeField] protected bool isGround = false;
    [SerializeField] protected float speed = 20f;
    [SerializeField] protected float lowSpeedCoef = 0.5f;
    [SerializeField] protected float highSpeedCoef = 1.5f;
    [SerializeField] protected float acceleration = 1f;
    [SerializeField] protected float minPursuitDist = 40f;
    [SerializeField] protected float maxPursuitDist = 50f;
    [SerializeField] protected float minAttackDist = 10f;
    [SerializeField] protected float maxAttackDist = 20f;
    [SerializeField] protected float distDelta = 1f;
    [SerializeField] protected GameObject deadPrefab;
    [SerializeField] protected GameObject explosion;

    protected NpcState npcState;
    protected NpcExplorer npcExplorer;
    protected NpcMoveToTgt npcMoveToTgt;
    protected NpcAttack npcAttack;
    protected Translation translation;
    protected Rotation rotation;
    protected Shooter shooter;
    protected Health health;
    protected GameObject selectedTarget;
    protected UnitController unitController;
    protected List<TargetTracker> trackers;

    #region Properties

    #region For change state

    protected bool EnemyForAttack => HorDistToTgt <= MinAttackDist; //7
    protected bool EnemyForPursuit => npcState == NpcState.Attack ?
        HorDistToTgt > MaxAttackDist : HorDistToTgt <= MinPursuitDist; //8
    protected bool EnemyLost => selectedTarget == null; //9
    protected Action<Caravan> AddToCaravanAction { get; set; }

    #endregion

    public Vector3 Position => transform.position;
    public GlobalSide2 Side => npcSide;
    public GameObject GameObject => gameObject;

    public bool IsFriendly
    {
        get => isFriendly;
        set => isFriendly = value;
    }
    public bool IsGround => isGround;
    public float Speed => speed;
    public float LowSpeed => speed * lowSpeedCoef;
    public float HighSpeed => speed * highSpeedCoef;
    public float Acceleration => acceleration;
    public float MinPursuitDist => minPursuitDist;
    public float MaxPursuitDist => maxPursuitDist;
    public float MinAttackDist => minAttackDist;
    public float MaxAttackDist => minPursuitDist;
    public float DistDelta => distDelta;
    public float HorDistToTgt
    {
        get
        {
            if (selectedTarget)
            {
                Vector3 toTgt = selectedTarget.transform.position - NpcPos;
                toTgt.y = 0f;
                return toTgt.magnitude;
            }
            else
                return Mathf.Infinity;
        }
    }
    public abstract Vector3 NpcPos { get; }
    public abstract Vector3 NpcCurrDir { get; }
    public GameObject SelectedTarget => selectedTarget;
    public Translation Translation => translation;
    public Rotation Rotation => rotation;

    #endregion

    protected void Init()
    {
        npcExplorer = GetComponent<NpcExplorer>();
        npcMoveToTgt = GetComponent<NpcMoveToTgt>();
        npcAttack = GetComponent<NpcAttack>();
        translation = GetComponent<Translation>();
        rotation = GetComponent<Rotation>();
        shooter = GetComponent<Shooter>();
        health = GetComponent<Health>();
        unitController = UnitController.Singleton;

        trackers = new List<TargetTracker>();
        trackers.AddRange(gameObject.GetComponentsInChildren<TargetTracker>());
    }

    public void SetTrackersRotation()
    {
        foreach (var track in trackers)
            track.SetRotation(selectedTarget, NpcCurrDir);
    }

    public void _SetSpeed(float speed) => this.speed = speed;

    public void AddToCaravan(Caravan caravan) => AddToCaravanAction?.Invoke(caravan);

    public abstract void RequestDestroy();
}
