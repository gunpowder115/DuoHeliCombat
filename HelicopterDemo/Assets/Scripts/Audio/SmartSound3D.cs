using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SmartSound3D : MonoBehaviour
{
    [SerializeField] private float maxHearingDistance = 30f;

    private AudioSource audioSource;
    private SmartSound3DController smartSound3DController;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource.spatialBlend != 0)
        {
            audioSource.spatialBlend = 0;
            Debug.LogWarning("Smart3DSound работает только с 2D AudioSource (Spatial Blend = 0)");
        }

        smartSound3DController = SmartSound3DController.Singleton;
        smartSound3DController.AddSound(this);
    }

    public void UpdateVolume(Player player1, Player player2)
    {
        float d1Sqr = player1 ? (transform.position - player1.transform.position).sqrMagnitude : Mathf.Infinity;
        float d2Sqr = player2 ? (transform.position - player2.transform.position).sqrMagnitude : Mathf.Infinity;
        float nearestSqr = Mathf.Min(d1Sqr, d2Sqr);

        audioSource.volume = Mathf.Clamp01(1f - nearestSqr / (maxHearingDistance * maxHearingDistance));
    }

    public void Play() => audioSource.Play();
}
