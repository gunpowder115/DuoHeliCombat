using UnityEngine;

public class BombingSpawner : MonoBehaviour
{
    [SerializeField] private float sideOffset = 5f;
    [SerializeField] private float deltaTime = 0.5f;
    [SerializeField] private GameObject bombPrefab;

    private bool isLeft;
    private float currTime;

    private void Update()
    {
        currTime += Time.deltaTime;
        if (currTime >= deltaTime)
        {
            var bombObject = Instantiate(bombPrefab, transform.position + new Vector3(0f, 0f, isLeft ? -sideOffset : sideOffset), transform.rotation);
            PickableUp pickableUp = bombObject.GetComponent<PickableUp>();
            Bomb bomb = bombObject.GetComponent<Bomb>();
            pickableUp.SetGravity(true);
            pickableUp.SetTrigger(false);
            bomb.IsActivated = true;
            bomb.IsBombing = true;
            isLeft = !isLeft;

            currTime = 0f;
        }
    }
}
