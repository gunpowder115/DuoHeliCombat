using System.Collections.Generic;
using UnityEngine;

public class FallenWalker : MonoBehaviour
{
    [SerializeField] private GameObject walkerModel;
    [SerializeField] private GameObject rotatingPart;
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;
    [SerializeField] private GameObject leftWeapon;
    [SerializeField] private GameObject rightWeapon;

    private float xPos;
    private Shooter shooter;
    private List<TargetTracker> targetTrackers;
    private UnitController unitController;
    private NpcGround npc;
    private bool init;

    public GameObject Target => unitController.FindClosestPlayer(npc).gameObject;

    private void Update()
    {
        if (!init)
        {
            unitController = UnitController.Singleton;
            npc = GetComponentInChildren<NpcGround>();
            shooter = GetComponentInChildren<Shooter>();
            shooter.BarrelFire(Target);
            init = true;
        }

        foreach (var tracker in targetTrackers)
            tracker.SetRotation(Target, tracker.transform.forward);
    }

    public void SetFallenParams(bool destroyingLeft, bool isLeftWeapon, in Quaternion headRotation, in Quaternion walkerRotation)
    {
        Destroy(destroyingLeft ? leftLeg : rightLeg);
        targetTrackers = new List<TargetTracker>();
        if (isLeftWeapon)
        {
            Destroy(rightWeapon);
            targetTrackers.AddRange(leftWeapon.GetComponentsInChildren<TargetTracker>());
        }
        else
        {
            Destroy(leftWeapon);
            targetTrackers.AddRange(rightWeapon.GetComponentsInChildren<TargetTracker>());
        }

        xPos = Mathf.Abs(leftLeg.transform.localPosition.x);
        walkerModel.transform.localPosition += new Vector3(destroyingLeft ? -xPos : xPos, 0f, 0f);
        rotatingPart.transform.localRotation = headRotation;
        transform.rotation = walkerRotation;
    }
}