using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private ParticleSystem explosionSystem;

    // Start is called before the first frame update
    void Start()
    {
        explosionSystem = GetComponent<ParticleSystem>();
        explosionSystem.Play();
        StartCoroutine(WaitForParticleSystemToStop(explosionSystem));
    }

    private IEnumerator WaitForParticleSystemToStop(ParticleSystem particleSystem)
    {
        while (particleSystem.isPlaying)
        {
            yield return null;
        }
        Destroy(gameObject);
    }
}
