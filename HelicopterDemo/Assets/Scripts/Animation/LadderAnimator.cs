using UnityEngine;

public class LadderAnimator : MonoBehaviour
{
    [SerializeField] private int ladderCount = 6;
    [SerializeField] private float partOffset = 0.6f;
    [SerializeField] private GameObject ladderPartPrefab;
    [SerializeField] private GameObject[] menPrefabs;

    private GameObject[] ladderParts;
    private GameObject[] men;

    void Start()
    {
        ladderParts = new GameObject[ladderCount];
        men = new GameObject[3];

        for (int i = 0; i < ladderCount; i++)
        {
            if (i == 0)
                ladderParts[i] = Instantiate(ladderPartPrefab, transform);
            else
            {
                ladderParts[i] = Instantiate(ladderPartPrefab, ladderParts[i - 1].transform);
                Vector3 pos = ladderParts[i].transform.position;
                ladderParts[i].transform.position = new Vector3(pos.x, pos.y - partOffset, pos.z);
            }
        }

        if (ladderParts.Length >= 3 && menPrefabs.Length >= 1) men[0] = Instantiate(menPrefabs[0], ladderParts[0].transform);
        if (ladderParts.Length >= 6 && menPrefabs.Length >= 1) men[1] = Instantiate(menPrefabs[1], ladderParts[3].transform);
        if (ladderParts.Length >= 9 && menPrefabs.Length >= 1) men[2] = Instantiate(menPrefabs[2], ladderParts[6].transform);



        for(int i = 0; i < men.Length; i++)
        {
            float xScale = Random.Range(0, 2) == 0 ? 1f : -1f;
            men[i].transform.localScale = new Vector3(xScale, 1f, i % 2 == 0 ? 1f : -1f);
        }
    }
}
