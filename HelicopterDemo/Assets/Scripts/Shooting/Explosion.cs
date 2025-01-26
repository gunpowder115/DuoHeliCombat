using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    void Start()
    {
        ParticleSystem[] parts = GetComponentsInChildren<ParticleSystem>();
        StartCoroutine(WaitForParticleSystemToStop(parts));
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
}
