using Assets.Scripts.Controllers;
using UnityEngine;
using static Types;

[RequireComponent(typeof(Rigidbody))]

public class PickableUp : MonoBehaviour, IFindable
{
    [SerializeField] private float takingDist = 5f;
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private PickableUpType pickableUpType;

    private Rigidbody rigidBody;
    private BoxCollider boxCollider;
    private UnitController unitController;
    private SmartSound3D pickSound;

    public Vector3 Position => transform.position;
    public GlobalSide2 Side => GlobalSide2.Red;
    public GameObject GameObject => gameObject;
    public PickableUpType PickableUpType => pickableUpType;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        pickSound = GetComponent<SmartSound3D>();
        unitController = UnitController.Singleton;
        SetGravity(false);
        SetTrigger(true);
    }

    private void Update()
    {
        float distToPlayer = Mathf.Infinity;
        PlayerBody playerBody = unitController.FindClosestPlayer(this, out distToPlayer).PlayerBody;

        if (distToPlayer < takingDist)
        {
            playerBody.ItemForTake = this;
        }
        else if (distToPlayer >= takingDist && playerBody.ItemForTake == this)
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
        pickSound.Clip = pickUpSound;
        pickSound.Loop = false;
        pickSound.Play();
    }
}
