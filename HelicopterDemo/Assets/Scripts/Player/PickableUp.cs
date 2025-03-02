using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PickableUp : MonoBehaviour
{
    [SerializeField] private float takingDist = 5f;

    private Rigidbody rigidBody;
    private BoxCollider boxCollider;
    private NpcController npcController;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        npcController = NpcController.Singleton;
        SetGravity(false);
        SetTrigger(true);
    }

    private void Update()
    {
        var player = npcController.FindNearestPlayer(transform.position);
        PlayerBody playerBody = player.Key.GetComponent<Player>().PlayerBody;
        float distPlayer = player.Value;

        if (distPlayer < takingDist)
        {
            playerBody.ItemForTake = this;
        }
        else
        {
            playerBody.ItemForTake = null;
        }
    }

    public void SetGravity(bool gravity)
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        rigidBody.useGravity = gravity;
    }

    public void SetTrigger(bool trigger) => boxCollider.isTrigger = trigger;
}
