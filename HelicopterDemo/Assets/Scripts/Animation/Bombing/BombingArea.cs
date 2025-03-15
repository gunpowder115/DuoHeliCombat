using UnityEngine;

public class BombingArea : MonoBehaviour
{
    [SerializeField] private float radius = 30f;
    [SerializeField] private float deltaTime = 0.5f;
    [SerializeField] private GameObject explosionPrefab;

    private float currTime;

    private void Update()
    {
        currTime += Time.deltaTime;

        if (currTime >= deltaTime)
        {

            Vector3 areaCenter = transform.position;
            areaCenter.y = 0f;

            float deltaX = Random.Range(-radius, radius);
            float deltaZ = Random.Range(-radius, radius);
            Vector3 delta = new Vector3(deltaX, 0f, deltaZ);

            Vector3 explosionPos = areaCenter + delta;

            if (explosionPrefab)
                Instantiate(explosionPrefab, explosionPos, transform.rotation);

            currTime = 0f;
        }
    }
}
