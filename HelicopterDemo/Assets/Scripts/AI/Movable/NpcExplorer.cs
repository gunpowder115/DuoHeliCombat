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

    private Vector3 initialDirection, avoidingOffset;
    private float sideToAvoid;
    private float currentSpeed;
    private float obstacleRadius;
    private GameObject obstacle;
    private AvoidingType avoidingType;

    private bool IsGround => npc.IsGround;
    private float Speed => npc.Speed;
    private float LowSpeed => npc.LowSpeed;
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
        avoidingType = AvoidingType.None;

        lineToTarget = GetComponent<LineRenderer>();
        if (!lineToTarget)
            lineToTarget = gameObject.AddComponent<LineRenderer>();
        lineToTarget.enabled = false;

        targetDirection = npc.NpcCurrDir;
        currentSpeed = Speed;
    }

    public void Move()
    {
        if (IsGround)
        {
            bool npcOut = npc.NpcPos.x > 256 || npc.NpcPos.x < -256 || npc.NpcPos.z > 128 || npc.NpcPos.z < -128;
            switch (avoidingType)
            {
                case AvoidingType.Wall:
                    NavigateFromWall();
                    break;
                case AvoidingType.Obstacle:
                    if (CheckWall(targetDirection)) 
                        obstacle = null;
                    else
                        NavigateAroundObstacle();
                    break;
                default:
                    if (CheckWall(targetDirection)) { }
                    else if (obstacle)
                        DetectNearObstacle();
                    else
                    {
                        SetDirection();
                        obstacle = DetectFarObstacle();
                    }
                    break;
            }
            npcSquad.MoveSquad(targetDirection, currentSpeed);
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
                targetHeight = transform.position.y;
            }
            else
            {
                targetDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
                targetHeight = IsGround ? 0f : Random.Range(MinHeight, MaxHeight);
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

    private void DrawLine(Color color, Vector3 endPoint)
    {
        lineToTarget.enabled = true;
        lineToTarget.startColor = color;
        lineToTarget.endColor = color;
        lineToTarget.SetPosition(0, npc.NpcPos);
        lineToTarget.SetPosition(1, npc.NpcPos + endPoint);
    }

    private GameObject DetectFarObstacle()
    {
        var raycastHits = Physics.SphereCastAll(npc.NpcPos + npc.NpcCurrDir, 5f, npc.NpcCurrDir, 5f);
        foreach (var hit in raycastHits)
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.GetComponent<CentralObstacle>() || hitObject.GetComponent<SideObstacle>())
            {
                obstacleRadius = (hitObject.transform.position - npc.NpcPos).magnitude - 5f;
                currentSpeed = LowSpeed;
                return hitObject;
            }
        }
        return null;
    }

    private void DetectNearObstacle()
    {
        if ((npc.NpcPos - obstacle.transform.position).magnitude < obstacleRadius + 3f)
        {
            initialDirection = npc.NpcCurrDir;
            GetSideOfAvoid(obstacle);
            avoidingType = AvoidingType.Obstacle;
        }
    }

    private void GetSideOfAvoid(GameObject obstacle)
    {
        var centralObst = obstacle.GetComponent<CentralObstacle>();
        var sideObst = obstacle.GetComponent<SideObstacle>();

        Vector3 toObstacle = obstacle.transform.position - npc.NpcPos;
        toObstacle.y = 0f;
        toObstacle.Normalize();

        if (centralObst)
            sideToAvoid = Vector3.Cross(npc.NpcCurrDir, toObstacle).y;
        else if (sideObst)
            sideToAvoid = Vector3.Cross(sideObst.ForwardDir, npc.NpcCurrDir).y;

        avoidingOffset = new Vector3(-sideToAvoid * toObstacle.z, 0f, sideToAvoid * toObstacle.x);
        targetDirection = avoidingOffset;
    }

    private void NavigateAroundObstacle()
    {
        Vector3 toObstacle = obstacle.transform.position - npc.NpcPos;
        toObstacle.y = 0f;
        toObstacle.Normalize();
        targetDirection = new Vector3(-sideToAvoid * toObstacle.z, 0f, sideToAvoid * toObstacle.x);

        if (Vector3.Angle(targetDirection, initialDirection) < 5f)
        {
            currentSpeed = Speed;
            avoidingType = AvoidingType.None;
            obstacle = null;
        }
    }

    private void NavigateFromWall()
    {
        if (Vector3.Angle(npc.NpcCurrDir, initialDirection) < 5f)
        {
            currentSpeed = Speed;
            obstacle = null;
            avoidingType = AvoidingType.None;
        }
    }

    private bool CheckWall(Vector3 dir)
    {
        var raycastHits = Physics.SphereCastAll(npc.NpcPos + dir, 5f, dir, 5f);
        foreach (var hit in raycastHits)
        {
            GameObject hitObject = hit.transform.gameObject;
            var wall = hitObject.GetComponent<Wall>();
            if (wall)
            {
                initialDirection = wall.ForwardDir;
                targetDirection = initialDirection;
                currentSpeed = LowSpeed;
                avoidingType = AvoidingType.Wall;
                return true;
            }
        }
        return false;
    }

    private enum AvoidingType
    {
        Wall,
        Obstacle,
        None
    }
}
