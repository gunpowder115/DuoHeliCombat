using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float damage = 1000f;
    [SerializeField] private GameObject explosion;

    public bool IsActivated { get; set; }
    public bool IsBombing { get; set; }

    private void Awake()
    {
        IsActivated = false;
        IsBombing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsBombing)        
            BombingActivation();
        else
            InGameActivation(collision);
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