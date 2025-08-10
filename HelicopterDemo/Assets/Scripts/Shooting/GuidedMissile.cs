using Assets.Scripts.Controllers;
using UnityEngine;
using static Types;

public class GuidedMissile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float minTrackingDist = 1f;
    [SerializeField] private float explosionForce = 1f;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioClip launchSound;
    [SerializeField] private AudioClip flyingSound;
    [SerializeField] private ParticleSystem smokeTail;

    private bool isLaunchSound;
    private float currLifetime;
    private AudioSource projSound;

    public bool IsPlayer { get; set; }
    public GameObject SelectedTarget { get; set; }
    public GlobalSide2 Side { get; set; }

    void Start()
    {
        projSound = GetComponent<AudioSource>();
        if (projSound)
        {
            projSound.pitch = Random.Range(0.6f, 0.8f);
            projSound.loop = false;
            projSound.clip = launchSound;
            projSound.Play();
            isLaunchSound = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currLifetime += Time.deltaTime;
        if (currLifetime > lifetime)
            Destroy(gameObject);

        if (SelectedTarget)
        {
            Vector3 dir = (SelectedTarget.transform.position - transform.position);
            Vector3 dirNorm = dir.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dirNorm);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            if (dir.magnitude < minTrackingDist)
                SelectedTarget = null;
        }
        transform.Translate(0f, 0f, speed * Time.deltaTime);

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

        if (explosion) Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
        if (smokeTail)
        {
            smokeTail.transform.parent = null;
            smokeTail.Stop();

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[smokeTail.particleCount];
            int count = smokeTail.GetParticles(particles);

            for (int i = 0; i < count; i++)
                particles[i].velocity = Vector3.zero;

            smokeTail.SetParticles(particles, count);
        }
        Destroy(gameObject);
    }

    public bool FriendlyFire(GlobalSide2 anotherSide) => Side == anotherSide;
}