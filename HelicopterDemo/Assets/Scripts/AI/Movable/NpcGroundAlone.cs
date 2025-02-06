using UnityEngine;

[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(Health))]

public class NpcGroundAlone : Npc
{
    public override Vector3 NpcPos => transform.position;
    public override Vector3 NpcCurrDir => rotation.CurrentDirection;
    public Npc AttackSource { get; private set; }

    private void Awake()
    {
        base.Init();
        npcController.Add(gameObject);
    }

    public void SetTarget(GameObject tgt) => selectedTarget = tgt;

    public override void RequestDestroy()
    {
        npcController.Remove(gameObject);

        if (deadPrefab)
            Instantiate(deadPrefab, transform.position, transform.rotation);

        if (explosion)
            Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);

        Destroy(gameObject);
    }
}
