using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float damage = 1000f;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioClip flyingSound;

    private AudioSource bombSound;

    public bool IsActivated { get; set; }
    public bool IsBombing { get; set; }

    private void Awake()
    {
        IsActivated = false;
        IsBombing = false;

        bombSound = GetComponent<AudioSource>();
        bombSound.loop = false;
        bombSound.clip = flyingSound;
        bombSound.volume = 0.7f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (bombSound.isPlaying)
            bombSound.Stop();

        if (IsBombing)        
            BombingActivation();
        else
            InGameActivation(collision);
    }

    public void PlaySound()
    {
        if (!bombSound.isPlaying)
            bombSound.Play();
    }

    private void InGameActivation(Collision collision)
    {
        if (IsActivated)
        {
            Building building = collision.gameObject.GetComponent<Building>();
            Health health = collision.gameObject.GetComponent<Health>();
            if (health && building)
            {
                health.Hurt(damage);
                if (explosion) Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    private void BombingActivation()
    {
        if (IsActivated)
        {
            if (explosion) Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);
        }
    }
}