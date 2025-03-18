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

    private float currentSpeed;
    private ObstacleAvoider obstacleAvoider;

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

        lineToTarget = GetComponent<LineRenderer>();
        if (!lineToTarget)
            lineToTarget = gameObject.AddComponent<LineRenderer>();
        lineToTarget.enabled = false;

        targetDirection = npc.NpcCurrDir;
        currentSpeed = Speed;

        obstacleAvoider = new ObstacleAvoider(SetDirection);
    }

    public void Move()
    {
        if (IsGround)
        {
            obstacleAvoider.GroundObstacleAvoid(npc.NpcPos, npc.NpcCurrDir, Speed, LowSpeed, ref targetDirection, ref currentSpeed);
            npcSquad.MoveSquad(targetDirection, currentSpeed);
        }
        else
        {
            obstacleAvoider.AirObstacleAvoid(npc.NpcPos, npc.NpcCurrDir, Speed, LowSpeed, ref targetDirection, ref currentSpeed);
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
}
