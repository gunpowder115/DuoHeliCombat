using UnityEngine;

public class NpcExplorer : MonoBehaviour
{
    [SerializeField] private float maxMoveTime = 10f;
    [SerializeField] private float stopTime = 1f;
    [SerializeField] private float absBorderX = 200f;
    [SerializeField] private float absBorderZ = 200f;

    private float currMoveTime, currStopTime;
    private float targetHeight, targetVerticalSpeed, currVerticalSpeed;
    private Vector3 targetSpeed, currSpeed;
    private Vector3 targetDirection;
    private Npc npc;
    private NpcAir npcAir;
    private NpcSquad npcSquad;
    private LineRenderer lineToTarget;
    private bool isAvoiding;

    private bool IsGround => npc.IsGround;
    private float Speed => npc.LowSpeed;
    private float VerticalSpeed => npcAir.VerticalSpeed;
    private float HeightDelta => npcAir.HeightDelta;
    private float Acceleration => npc.Acceleration;
    private float MinHeight => npcAir.MinHeight;
    private float MaxHeight => npcAir.MaxHeight;
    private Translation Translation => npc.Translation;
    private Rotation Rotation => npc.Rotation;

    void Start()
    {
        npc = GetComponent<Npc>();
        npcAir = GetComponent<NpcAir>();
        npcSquad = GetComponent<NpcSquad>();
        currMoveTime = maxMoveTime;

        lineToTarget = GetComponent<LineRenderer>();
        if (!lineToTarget)
            lineToTarget = gameObject.AddComponent<LineRenderer>();
        lineToTarget.enabled = false;
    }

    public void Move()
    {
        SetDirection();
        if (CheckObstacles())
            currMoveTime = maxMoveTime;

        if (IsGround)
        {
            if (!isAvoiding)
            {
                GameObject obstacle = CheckStraightObstacle();
                if (obstacle)
                {
                    npcSquad._SetSpeed(0f);
                    //targetDirection = GetObstacleAvoiding(obstacle);
                    isAvoiding = true;
                }
            }

            //DrawLine();
            npcSquad.MoveSquad(targetDirection, Speed);

        }
        else
        {
            TranslateAir();
            VerticalTranslate();
            RotateAir();
        }
    }

    private void TranslateAir()
    {
        targetSpeed = Vector3.ClampMagnitude(targetDirection * Speed, Speed);
        currSpeed = Vector3.Lerp(currSpeed, targetSpeed, Acceleration * Time.deltaTime);
        Translation.SetHorizontalTranslation(currSpeed);
    }

    private void RotateAir()
    {
        var direction = targetDirection != Vector3.zero ? targetDirection : Rotation.CurrentDirection;
        var speedCoef = targetDirection != Vector3.zero ? currSpeed.magnitude / Speed : 0f;
        Rotation.RotateToDirection(direction, speedCoef, true);
    }

    private void VerticalTranslate()
    {
        float vertDir = 0f;
        if (Mathf.Abs(targetHeight - transform.position.y) > HeightDelta)
            vertDir = targetHeight > transform.position.y ? 1f : -1f;
        targetVerticalSpeed = vertDir * VerticalSpeed;
        currVerticalSpeed = Mathf.Lerp(currVerticalSpeed, targetVerticalSpeed, Acceleration * Time.deltaTime);
        Translation.SetVerticalTranslation(currVerticalSpeed);
    }

    private void SetDirection()
    {
        if (currMoveTime >= maxMoveTime)
        {
            if (Wait())
            {
                //targetDirection = Vector2.zero;
                targetHeight = transform.position.y;
            }
            else
            {
                do
                {
                    targetDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
                    targetHeight = IsGround ? 0f : Random.Range(MinHeight, MaxHeight);
                    CheckBorders();
                } while (CheckObstacles());
                currMoveTime = 0f;
            }
        }
        else
            currMoveTime += Time.deltaTime;
    }

    private bool Wait()
    {
        if (currStopTime >= stopTime)
        {
            currStopTime = 0f;
            return false;
        }
        else
            currStopTime += Time.deltaTime;
        return true;
    }

    private bool CheckObstacles()
    {
        var raycastHits = Physics.SphereCastAll(npc.NpcPos + npc.NpcCurrDir * 5, 8f, targetDirection, 20f);
        for (int i = 0; i < raycastHits.Length; i++)
        {
            var hit = raycastHits[i];
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.CompareTag("Obstacle"))
                return true;
        }
        return false;
    }

    private void CheckBorders()
    {
        if (transform.position.x > absBorderX || transform.position.x < -absBorderX)
            targetDirection = new Vector3(-targetDirection.x, targetDirection.y, targetDirection.z);

        if (transform.position.z > absBorderZ || transform.position.z < -absBorderZ)
            targetDirection = new Vector3(targetDirection.x, targetDirection.y, -targetDirection.z);
    }

    private void DrawLine(Color color, Vector3 endPoint)
    {
        lineToTarget.enabled = true;
        lineToTarget.startColor = color;
        lineToTarget.endColor = color;
        lineToTarget.SetPosition(0, npc.NpcPos);
        lineToTarget.SetPosition(1, npc.NpcPos + endPoint);
    }

    private GameObject CheckStraightObstacle()
    {
        var raycastHits = Physics.SphereCastAll(npc.NpcPos + npc.NpcCurrDir, 5f, targetDirection, 5f);
        foreach (var hit in raycastHits)
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.GetComponent<CentralObstacle>())
                return hitObject;
        }
        return null;
    }

    private Vector3 GetObstacleAvoiding(GameObject obstacle)
    {
        Vector3 toObstacle = obstacle.transform.position - npc.NpcPos;
        toObstacle.y = 0f;
        toObstacle.Normalize();

        float proj = Vector3.Dot(toObstacle, npc.NpcCurrDir);
        Vector3 vecProj = proj * npc.NpcCurrDir;
        Vector3 turnDir = vecProj - toObstacle;
        turnDir.Normalize();

        return turnDir;
    }
}
