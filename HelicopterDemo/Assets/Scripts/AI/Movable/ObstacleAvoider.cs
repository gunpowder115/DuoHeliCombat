using System;
using UnityEngine;

public class ObstacleAvoider
{
    private float obstacleRadius;
    private float sideToAvoid;
    private Vector3 initialDirection, avoidingOffset;
    private GameObject obstacle;
    private AvoidingType avoidingType;
    private Action setDirAction;

    private float speed, lowSpeed;
    private Vector3 npcPos, npcCurrDir;

    public ObstacleAvoider(Action setDirAction)
    {
        avoidingType = AvoidingType.None;
        this.setDirAction = setDirAction;
    }

    public void GroundObstacleAvoid(Vector3 pos, Vector3 currDir, float speed, float lowSpeed, ref Vector3 tgtDir, ref float currSpeed)
    {
        this.speed = speed;
        this.lowSpeed = lowSpeed;
        npcPos = pos;
        npcCurrDir = currDir;

        switch (avoidingType)
        {
            case AvoidingType.Wall:
                NavigateFromWall(ref currSpeed);
                break;
            case AvoidingType.Obstacle:
                if (CheckWall(tgtDir, ref tgtDir, ref currSpeed))
                    obstacle = null;
                else
                    NavigateAroundObstacle(ref tgtDir, ref currSpeed);
                break;
            default:
                if (CheckWall(tgtDir, ref tgtDir, ref currSpeed)) { }
                else if (obstacle)
                    DetectNearObstacle(ref tgtDir);
                else
                {
                    setDirAction?.Invoke();
                    obstacle = DetectFarObstacle(ref currSpeed);
                }
                break;
        }
    }

    public void AirObstacleAvoid(Vector3 pos, Vector3 currDir, float speed, float lowSpeed, ref Vector3 tgtDir, ref float currSpeed)
    {
        this.speed = speed;
        this.lowSpeed = lowSpeed;
        npcPos = pos;
        npcCurrDir = currDir;

        switch (avoidingType)
        {
            case AvoidingType.Wall:
                NavigateFromWall(ref currSpeed);
                break;
            default:
                if (CheckWall(tgtDir, ref tgtDir, ref currSpeed)) { }
                else
                    setDirAction?.Invoke();
                break;
        }
    }

    private GameObject DetectFarObstacle(ref float currSpeed)
    {
        var raycastHits = Physics.SphereCastAll(npcPos + npcCurrDir, 5f, npcCurrDir, 5f);
        foreach (var hit in raycastHits)
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.GetComponent<CentralObstacle>() || hitObject.GetComponent<SideObstacle>())
            {
                obstacleRadius = (hitObject.transform.position - npcPos).magnitude - 5f;
                currSpeed = lowSpeed;
                return hitObject;
            }
        }
        return null;
    }

    private void DetectNearObstacle(ref Vector3 tgtDir)
    {
        if ((npcPos - obstacle.transform.position).magnitude < obstacleRadius + 3f)
        {
            initialDirection = npcCurrDir;
            GetSideOfAvoid(ref tgtDir);
            avoidingType = AvoidingType.Obstacle;
        }
    }

    private void NavigateFromWall(ref float currSpeed)
    {
        if (Vector3.Angle(npcCurrDir, initialDirection) < 5f)
        {
            currSpeed = speed;
            obstacle = null;
            avoidingType = AvoidingType.None;
        }
    }

    private void NavigateAroundObstacle(ref Vector3 tgtDir, ref float currSpeed)
    {
        Vector3 toObstacle = obstacle.transform.position - npcPos;
        toObstacle.y = 0f;
        toObstacle.Normalize();
        tgtDir = new Vector3(-sideToAvoid * toObstacle.z, 0f, sideToAvoid * toObstacle.x);

        if (Vector3.Angle(tgtDir, initialDirection) < 5f)
        {
            currSpeed = speed;
            avoidingType = AvoidingType.None;
            obstacle = null;
        }
    }

    private bool CheckWall(Vector3 checkDir, ref Vector3 tgtDir, ref float currSpeed)
    {
        var raycastHits = Physics.SphereCastAll(npcPos + checkDir, 5f, checkDir, 5f);
        foreach (var hit in raycastHits)
        {
            GameObject hitObject = hit.transform.gameObject;
            var wall = hitObject.GetComponent<Wall>();
            if (wall)
            {
                initialDirection = wall.ForwardDir;
                tgtDir = initialDirection;
                currSpeed = lowSpeed;
                avoidingType = AvoidingType.Wall;
                return true;
            }
        }
        return false;
    }

    private void GetSideOfAvoid(ref Vector3 tgtDir)
    {
        var centralObst = obstacle.GetComponent<CentralObstacle>();
        var sideObst = obstacle.GetComponent<SideObstacle>();

        Vector3 toObstacle = obstacle.transform.position - npcPos;
        toObstacle.y = 0f;
        toObstacle.Normalize();

        if (centralObst)
            sideToAvoid = Vector3.Cross(npcCurrDir, toObstacle).y;
        else if (sideObst)
            sideToAvoid = Vector3.Cross(sideObst.ForwardDir, npcCurrDir).y;

        avoidingOffset = new Vector3(-sideToAvoid * toObstacle.z, 0f, sideToAvoid * toObstacle.x);
        tgtDir = avoidingOffset;
    }

    private enum AvoidingType
    {
        Wall,
        Obstacle,
        None
    }
}