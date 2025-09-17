using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private bool shaking = false;
    [SerializeField] private float maxExplosionShakeDist = 15f;
    [SerializeField] private ExplosionType explosionType = ExplosionType.Death;

    private SmartSound3D explosionSound;
    private UnitController unitController;

    void Start()
    {
        unitController = UnitController.Singleton;

        ParticleSystem[] parts = GetComponentsInChildren<ParticleSystem>();
        StartCoroutine(WaitForParticleSystemToStop(parts));

        explosionSound = GetComponent<SmartSound3D>();
        if (explosionSound)
        {
            switch (explosionType)
            {
                case ExplosionType.UnguidMissile:
                    explosionSound.Pitch = Random.Range(2f, 2.3f);
                    break;
                case ExplosionType.GuidMissile:
                    explosionSound.Pitch = Random.Range(1.3f, 1.5f);
                    break;
                case ExplosionType.CannonHit:
                    explosionSound.Pitch = Random.Range(0.7f, 0.9f);
                    break;
                case ExplosionType.Death:
                    explosionSound.Pitch = Random.Range(1f, 1.2f);
                    break;
                default:
                    explosionSound.Pitch = 1f;
                    break;
            }
            explosionSound.Play();
        }

        if (shaking)
        {
            foreach (var player in unitController.Players)
            {
                if (Mathf.Abs(player.Position.x - transform.position.x) < maxExplosionShakeDist &&
                    Mathf.Abs(player.Position.y - transform.position.y) < maxExplosionShakeDist &&
                    Mathf.Abs(player.Position.z - transform.position.z) < maxExplosionShakeDist)
                {
                    float dist = (player.Position - transform.position).magnitude;
                    (player as Player).HitForce = 1f - dist / maxExplosionShakeDist;
                }
            }
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
