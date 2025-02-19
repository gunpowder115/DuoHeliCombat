using UnityEngine;

public class FlagAnimator : MonoBehaviour
{
    [SerializeField] private float partOffset = 0.6f;
    [SerializeField] private GameObject[] flagPartsPrefabs;

    private int partCount;
    private GameObject[] flagParts;

    void Start()
    {
        partCount = flagPartsPrefabs.Length;
        flagParts = new GameObject[partCount];

        for (int i = 0; i < partCount; i++)
        {
            if (i == 0)
                flagParts[i] = Instantiate(flagPartsPrefabs[i], transform);
            else
            {
                flagParts[i] = Instantiate(flagPartsPrefabs[i], flagParts[i - 1].transform);
                Vector3 pos = flagParts[i].transform.localPosition;
                flagParts[i].transform.localPosition = new Vector3(pos.x + partOffset, pos.y, pos.z);
            }
        }
    }
}
