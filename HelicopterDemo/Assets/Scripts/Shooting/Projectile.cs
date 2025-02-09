using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10.0f;
    [SerializeField] float lifetime = 10.0f;
    [SerializeField] float damage = 5.0f;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioClip launchSound;
    [SerializeField] private AudioClip flyingSound;
    [SerializeField] private ProjectileType projectileType = ProjectileType.Minigun;

    private bool isLaunchSound;
    float currLifetime;
    AudioSource projSound;

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
        Health health = other.GetComponent<Health>();
        if (!FriendlyFire(other.gameObject.tag) && health)
        {
            bool damageFromPlayer = gameObject.tag == "Player";
            health.Hurt(damage, damageFromPlayer, other.GetComponent<Npc>());
        }

        if (explosion) Instantiate(explosion, gameObject.transform.position + transform.forward, gameObject.transform.rotation);
        Destroy(gameObject);
    }

    private bool FriendlyFire(string anotherTag) //todo remove tags
    {
        string thisTag = gameObject.tag;
        bool bothIsFriendly = (thisTag.Contains("Friendly") || thisTag.Contains("Player")) && (anotherTag.Contains("Friendly") || anotherTag.Contains("Player"));
        bool isEnemy = thisTag.Contains("Enemy") && anotherTag.Contains("Enemy");

        return bothIsFriendly || isEnemy;
    }

    private enum ProjectileType
    {
        Minigun,
        Cannon,
        UnguidMissile
    }
}
