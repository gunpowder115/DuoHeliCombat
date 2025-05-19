using System.Collections.Generic;
using UnityEngine;

public class Tether : MonoBehaviour
{
    [SerializeField] private Player heavyPlayer;
    [SerializeField] private Player lightPlayer;
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private int segmentCount = 20;
    [SerializeField] private float segmentLength = 0.5f;
    [SerializeField] private float maxTetherDistCoef = 0.8f;
    [SerializeField] private float maxDistToScanSegmentsCollides = 10f;
    [SerializeField] private float maxAngleForCoDir = 30f;
    [SerializeField] private float timeToDestroyByTether = 2f;
    [SerializeField] private float heightDeltaForTaut = 1f;

    private List<GameObject> segments = new List<GameObject>();
    private float maxTetherDist;
    private CamerasController camerasController;
    private float currDist;
    private Vector3 originPos;
    private DestroyableByTetherController fuelTowersController;
    private float collideTime;
    private FuelTower nearFuelTower;
    private Walker nearWalker;
    private float averageHeight;
    private bool isTaut;
    private bool collidesFuelTower, collidesWalker;

    private Transform heavyPoint => heavyPlayer.transform;
    private Transform lightPoint => lightPlayer.transform;

    private void Start()
    {
        Vector3 ropeDirection = (lightPoint.position - heavyPoint.position).normalized;
        Vector3 spacing = ropeDirection * segmentLength;

        Vector3 currentPos = heavyPoint.position;

        GameObject previous = null;

        for (int i = 0; i < segmentCount; i++)
        {
            Quaternion rotation = Quaternion.LookRotation(ropeDirection) * Quaternion.Euler(90, 0, 0);
            GameObject segment = Instantiate(segmentPrefab, currentPos, Quaternion.identity, transform);
            segments.Add(segment);

            Rigidbody rb = segment.GetComponent<Rigidbody>();

            if (i == 0)
            {
                ConfigurableJoint joint = segment.AddComponent<ConfigurableJoint>();
                joint.connectedBody = heavyPoint.GetComponent<Rigidbody>();
                ConfigureJoint(joint);
            }
            else
            {
                ConfigurableJoint joint = segment.AddComponent<ConfigurableJoint>();
                joint.connectedBody = previous.GetComponent<Rigidbody>();
                ConfigureJoint(joint);
            }

            currentPos += spacing;
            previous = segment;
        }

        ConfigurableJoint endJoint = segments[segments.Count - 1].AddComponent<ConfigurableJoint>();
        endJoint.connectedBody = lightPoint.GetComponent<Rigidbody>();
        ConfigureJoint(endJoint);

        for (int i = 0; i < segments.Count - 1; i++)
        {
            var visual = segments[i].GetComponent<TetherSegmentVisual>();
            visual.Target = segments[i + 1].transform;
        }
        segments[segments.Count - 1].GetComponent<TetherSegmentVisual>().Target = lightPoint;

        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].GetComponent<TetherSegmentVisual>().IsVisible = i < segments.Count - 1;
        }

        maxTetherDist = segmentLength * segmentCount * maxTetherDistCoef;

        camerasController = CamerasController.Singleton;
        fuelTowersController = DestroyableByTetherController.Singleton;
    }

    private void Update()
    {
        currDist = Vector3.Distance(heavyPoint.position, lightPoint.position);
        originPos = (lightPoint.position + heavyPoint.position) / 2f;
        GetAverageHeight();
        isTaut = Mathf.Abs(averageHeight - heavyPoint.position.y) < heightDeltaForTaut;

        camerasController.SetCamerasZoomOut(currDist, maxTetherDist);

        collidesFuelTower = TetherCollidesWithFuelTower();
        collidesWalker = TetherCollidesWithWalker();

        if (collidesFuelTower && isTaut && PlayersAreCoDir())
        {
            collideTime += Time.deltaTime;
            if (collideTime > timeToDestroyByTether)
                fuelTowersController.DestroyItem(nearFuelTower);
        }
        else if (collidesWalker && isTaut && PlayersAreCoDir())
        {
            collideTime += Time.deltaTime;
            if (collideTime > timeToDestroyByTether)
                fuelTowersController.DestroyItem(nearWalker);
        }
        else
            collideTime = 0f;

        //Debug.Log(isTaut);
    }

    private void FixedUpdate()
    {
        CheckIncorrectMovement();
    }

    private void ConfigureJoint(ConfigurableJoint joint)
    {
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = Vector3.zero;

        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit linearLimit = new SoftJointLimit();
        linearLimit.limit = segmentLength;
        joint.linearLimit = linearLimit;

        JointDrive drive = new JointDrive();
        drive.positionSpring = 0;
        drive.positionDamper = 50;
        drive.maximumForce = Mathf.Infinity;

        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;
    }

    private void CheckIncorrectMovement()
    {
        Vector3 speedHeavy = heavyPlayer.PlayerTranslation.Movement;
        Vector3 speedLight = lightPlayer.PlayerTranslation.Movement;
        Vector3 dirLightToHeavy = (heavyPoint.position - lightPoint.position).normalized;

        if ((collidesFuelTower || collidesWalker) && isTaut)
        {
            lightPlayer.PlayerTranslation.IsTowing = heavyPlayer.PlayerTranslation.IsTowing = true;
            lightPlayer.PlayerTranslation.TowingCoef = heavyPlayer.PlayerTranslation.TowingCoef = 0.5f;
        }
        else if (currDist > maxTetherDist)
        {
            if (speedHeavy == Vector3.zero && speedLight != Vector3.zero)
            {
                //float dot = Vector3.Dot(dirLightToHeavy, speedLight);
                //lightPlayer.PlayerTranslation.IsTowing = dot < 0f;
                //lightPlayer.PlayerTranslation.TowingCoef = 0.5f;
            }
            else if (speedHeavy != Vector3.zero && speedLight == Vector3.zero)
            {
            }
            else
            {
                float dot = Vector3.Dot(speedHeavy, speedLight);
                lightPlayer.PlayerTranslation.IsTowing = dot < 0f;
                lightPlayer.PlayerTranslation.TowingCoef = 0f;
            }
        }
        else
        {
            lightPlayer.PlayerTranslation.IsTowing = false;
            heavyPlayer.PlayerTranslation.IsTowing = false;
        }
    }

    private bool TetherCollidesWithFuelTower()
    {
        nearFuelTower = fuelTowersController.GetNearFuelTower(originPos, maxDistToScanSegmentsCollides);
        if (nearFuelTower)
        {
            foreach (var item in segments)
            {
                if (item.GetComponent<TetherSegmentVisual>().CollidesWithItem)
                    return true;
            }
        }
        return false;
    }

    private bool TetherCollidesWithWalker()
    {
        nearWalker = fuelTowersController.GetNearWalker(originPos, maxDistToScanSegmentsCollides);
        if (nearWalker)
        {
            nearWalker.StopWalker();
            foreach (var item in segments)
            {
                if (item.GetComponent<TetherSegmentVisual>().CollidesWithItem)
                    return true;
            }
        }
        return false;
    }

    private bool PlayersAreCoDir()
    {
        Vector3 speedHeavy = heavyPlayer.PlayerTranslation.Movement;
        Vector3 speedLight = lightPlayer.PlayerTranslation.Movement;
        return Vector3.Angle(speedHeavy, speedLight) < maxAngleForCoDir;
    }

    private void GetAverageHeight()
    {
        averageHeight = 0f;
        foreach (var item in segments)
        {
            averageHeight += item.transform.position.y;
        }
        averageHeight /= segmentCount;
    }
}