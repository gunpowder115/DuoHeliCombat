using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float damage = 1000f;
    [SerializeField] private GameObject explosion;
    [SerializeField] private AudioClip flyingSound;

    private SmartSound3D bombSound;

    public bool IsActivated { get; set; }
    public bool IsBombing { get; set; }

    private void Awake()
    {
        IsActivated = false;
        IsBombing = false;

        bombSound = GetComponent<SmartSound3D>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (bombSound.IsPlaying)
            bombSound.Stop();

        if (IsBombing)        
            BombingActivation();
        else
            InGameActivation(collision);
    }

    public void PlaySound()
    {
        bombSound.Loop = false;
        bombSound.Clip = flyingSound;
        if (!bombSound.IsPlaying)
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