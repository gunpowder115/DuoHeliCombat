using UnityEngine;

[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Translation))]
[RequireComponent(typeof(Rotation))]

public class NpcGround : Npc
{
    public bool UnderAttack
    {
        get
        {
            AttackSource = health.AttackSource;
            return health.IsUnderAttack;
        }
        set => health.IsUnderAttack = value;
    }
    public bool BehindSquad { get; set; }
    public bool FarFromSquad { get; set; }
    public Vector3 CurrentSpeed { get; private set; }
    public override Vector3 NpcPos => transform.position;
    public override Vector3 NpcCurrDir => rotation.CurrentDirection;
    public Npc AttackSource { get; private set; }
    public NpcSquad NpcSquad { get; set; }

    private void Awake()
    {
        base.Init();
    }

    public void SetTarget(GameObject tgt) => selectedTarget = tgt;

    public void Translate(Vector3 targetSpeed)
    {
        CurrentSpeed = Vector3.Lerp(CurrentSpeed, targetSpeed, Acceleration * Time.deltaTime);
        translation.SetHorizontalTranslation(CurrentSpeed);
    }

    public void Drop(float speed) => translation.SetVerticalTranslation(speed);

    public GameObject GetNextMember()
    {
        if (NpcSquad.Members.Count <= 1)
            return null;
        else
        {
            int currIndex = NpcSquad.Members.IndexOf(gameObject);
            currIndex++;
            if (currIndex >= NpcSquad.Members.Count) currIndex = 0;
            return NpcSquad.Members[currIndex];
        }
    }

    public override void RequestDestroy()
    {
        npcController.Remove(gameObject);
        bool isSquad = NpcSquad.RemoveMember(this);

        if (deadPrefab)
        {
            var deadNpc = Instantiate(deadPrefab, transform.position, transform.rotation);

            var currTrackers = GetComponentsInChildren(typeof(TargetTracker));
            Vector3 towerAngle = (currTrackers[0] as TargetTracker).gameObject.transform.rotation.eulerAngles;
            Vector3 barrelAngle = (currTrackers[1] as TargetTracker).gameObject.transform.rotation.eulerAngles;

            var trackers = deadNpc.GetComponentsInChildren(typeof(TargetTracker));
            (trackers[0] as TargetTracker).SetRotation(null, towerAngle);
            (trackers[1] as TargetTracker).SetRotation(null, barrelAngle);
        }

        if (explosion)
            Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);

        Destroy(gameObject);
        if (!isSquad)
            Destroy(NpcSquad.gameObject);
    }

    public bool FarFrom(NpcGround npc, float dist) => Vector3.Magnitude(transform.position - npc.gameObject.transform.position) > dist;
    public bool FarFrom(NpcGround npc1, NpcGround npc2, float dist) => FarFrom(npc1, dist) && FarFrom(npc2, dist);
}
