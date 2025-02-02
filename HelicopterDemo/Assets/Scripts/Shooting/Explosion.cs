using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private ExplosionType explosionType = ExplosionType.Death;

    private AudioSource explosionSound;

    void Start()
    {
        ParticleSystem[] parts = GetComponentsInChildren<ParticleSystem>();
        StartCoroutine(WaitForParticleSystemToStop(parts));

        explosionSound = GetComponent<AudioSource>();
        if (explosionSound)
        {
            switch(explosionType)
            {
                case ExplosionType.UnguidMissile:
                    explosionSound.pitch = Random.Range(2f, 2.3f);
                    break;
                case ExplosionType.GuidMissile:
                    explosionSound.pitch = Random.Range(1.3f, 1.5f);
                    break;
                case ExplosionType.CannonHit:
                    explosionSound.pitch = Random.Range(0.7f, 0.9f);
                    break;
                case ExplosionType.Death:
                    explosionSound.pitch = Random.Range(1f, 1.2f);
                    break;
                default:
                    explosionSound.pitch = 1f;
                    break;
            }
            explosionSound.Play();
        }
    }

    private IEnumerator WaitForParticleSystemToStop(ParticleSystem[] parts)
    {
        while (parts[0].isPlaying)
        {
            yield return null;
        }

        foreach (var part in parts)
            Destroy(part.gameObject);
        Destroy(gameObject);
    }

    private enum ExplosionType
    {
        GuidMissile,
        UnguidMissile,
        Death,
        CannonHit,
        ProjectileHit
    }
}
