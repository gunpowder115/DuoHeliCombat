using UnityEngine;

public class LadderAnimator : MonoBehaviour
{
    [SerializeField] private bool ladderActive = false;
    [SerializeField] private bool ladderExit = false;
    [SerializeField] private int ladderCount = 6;
    [SerializeField] private float partOffset = 0.6f;
    [SerializeField] private float exitSpeed = 1f;
    [SerializeField] private GameObject ladderPartPrefab;
    [SerializeField] private GameObject[] menPrefabs;

    private GameObject[] ladderParts;
    private GameObject[] men;
    private Renderer[] rends;
    private Vector3 speed;
    private Vector3 targetPos, currentPos;
    private bool menInitialized, ladderInitialized;
    private int currentPart;

    public bool EndOfLadderExit { get; private set; }

    private void Update()
    {
        if (ladderActive)
        {
            if (!ladderInitialized)
            {
                InitLadder();

                rends = GetComponentsInChildren<Renderer>();

                for (int i = 0; i < rends.Length - 1; i++)
                    rends[i].enabled = false;
                targetPos = transform.localPosition;
                transform.Translate(new Vector3(0f, partOffset * ladderCount, 0f));
                currentPos = transform.localPosition;
                currentPart = rends.Length - 2;
                speed = new Vector3(0f, exitSpeed, 0f);

                ladderInitialized = true;
            }

            transform.Translate(-speed * Time.deltaTime);

            if (currentPos.y - transform.localPosition.y > partOffset && currentPart >= 0)
            {
                rends[currentPart--].enabled = true;
                currentPos = transform.localPosition;
            }

            if (transform.localPosition.y <= targetPos.y)
            {
                transform.localPosition = targetPos;
                if (!menInitialized) InitMenOnLadder();
                EndOfLadderExit = false;
            }
        }
    }

    public void ExitLadder() => ladderActive = true;

    private void InitLadder()
    {
        ladderParts = new GameObject[ladderCount];

        for (int i = 0; i < ladderCount; i++)
        {
            if (i == 0)
                ladderParts[i] = Instantiate(ladderPartPrefab, transform);
            else
            {
                ladderParts[i] = Instantiate(ladderPartPrefab, ladderParts[i - 1].transform);
                Vector3 pos = ladderParts[i].transform.localPosition;
                ladderParts[i].transform.localPosition = new Vector3(pos.x, pos.y - partOffset, pos.z);
            }
        }
    }

    private void InitMenOnLadder()
    {
        men = new GameObject[3];

        if (ladderParts.Length >= 3 && menPrefabs.Length >= 1) men[0] = Instantiate(menPrefabs[0], ladderParts[0].transform);
        if (ladderParts.Length >= 6 && menPrefabs.Length >= 1) men[1] = Instantiate(menPrefabs[1], ladderParts[3].transform);
        if (ladderParts.Length >= 9 && menPrefabs.Length >= 1) men[2] = Instantiate(menPrefabs[2], ladderParts[6].transform);

        for (int i = 0; i < men.Length; i++)
        {
            float xScale = Random.Range(0, 2) == 0 ? 0.5f : -0.5f;
            if (men[i]) men[i].transform.localScale = new Vector3(xScale, 0.5f, i % 2 == 0 ? 0.5f : -0.5f);
        }

        menInitialized = true;
    }
}
