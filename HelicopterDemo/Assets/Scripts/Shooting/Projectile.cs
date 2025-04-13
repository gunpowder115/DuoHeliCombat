using Assets.Scripts.Controllers;
using UnityEngine;
using static Types;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float lifetime = 10.0f;
    [SerializeField] private float damage = 5.0f;
    [SerializeField] private float explosionForce = 1f;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioClip launchSound;
    [SerializeField] private AudioClip flyingSound;
    [SerializeField] private ProjectileType projectileType = ProjectileType.Minigun;

    private bool isLaunchSound;
    float currLifetime;
    AudioSource projSound;

    public bool IsPlayer { get; set; }
    public float ExplosionForce => explosionForce;
    public GlobalSide2 Side { get; set; }

    void Start()
    {
        currLifetime = 0.0f;
        projSound = GetComponent<AudioSource>();
        if (projSound)
        {
            switch (projectileType)
            {
                case ProjectileType.UnguidMissile:
                    projSound.pitch = Random.Range(1.3f, 1.5f);
                    break;
                case ProjectileType.Cannon:
                    projSound.pitch = Random.Range(0.7f, 0.9f);
                    break;
                case ProjectileType.Minigun:
                    projSound.pitch = Random.Range(0.5f, 0.6f);
                    break;
                default:
                    projSound.pitch = 1f;
                    break;
            }
            projSound.loop = false;
            projSound.clip = launchSound;
            projSound.Play();
            isLaunchSound = true;
        }
    }

    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
        currLifetime += Time.deltaTime;

        if (currLifetime >= lifetime)
            Destroy(this.gameObject);

        if (isLaunchSound && !projSound.isPlaying && flyingSound)
        {
            projSound.loop = true;
            projSound.clip = flyingSound;
            projSound.Play();
            isLaunchSound = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        IFindable otherFindable = other.GetComponent<IFindable>();
        IFindable otherParentFindable = other.transform?.parent?.GetComponent<IFindable>();
        Health health = other.GetComponent<Health>();
        if (otherFindable != null && !FriendlyFire(otherFindable.Side) && health)
        {
            health.Hurt(damage, IsPlayer, other.GetComponent<Npc>());
        }
        else if (otherParentFindable != null && !FriendlyFire(otherParentFindable.Side) && health)
        {
            health.Hurt(damage, IsPlayer, other.GetComponent<Npc>());
            (otherParentFindable as Player).HitForce = explosionForce;
        }

        if (explosion) Instantiate(explosion, gameObject.transform.position + transform.forward, gameObject.transform.rotation);
        Destroy(gameObject);
    }

    public bool FriendlyFire(GlobalSide2 anotherSide) => Side == anotherSide;

    private enum ProjectileType
    {
        Minigun,
        Cannon,
        UnguidMissile
    }
}
