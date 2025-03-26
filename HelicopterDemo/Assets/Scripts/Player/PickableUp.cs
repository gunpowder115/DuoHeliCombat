using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PickableUp : MonoBehaviour
{
    [SerializeField] private float takingDist = 5f;
    [SerializeField] private AudioClip pickUpSound;

    private Rigidbody rigidBody;
    private BoxCollider boxCollider;
    private NpcController npcController;
    private AudioSource sound;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        sound = GetComponent<AudioSource>();
        npcController = NpcController.Singleton;
        SetGravity(false);
        SetTrigger(true);

        sound.loop = false;
        sound.clip = pickUpSound;
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
        else if (distPlayer >= takingDist && playerBody.ItemForTake == this)
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

    public void SetTrigger(bool trigger)
    {
        if (boxCollider) boxCollider.isTrigger = trigger;
    }

    public void PlaySound()
    {
        sound.clip = pickUpSound;
        sound.volume = 1f;
        sound.Play();
    }
}
