using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SmartSound3D : MonoBehaviour
{
    [SerializeField] private float maxHearingDistance = 30f;

    private AudioSource audioSource;
    private SmartSound3DController smartSound3DController;

    private Player player1 => smartSound3DController.Player1;
    private Player player2 => smartSound3DController.Player2;

    #region Properties

    public float Pitch
    {
        get => audioSource.pitch;
        set => audioSource.pitch = value;
    }
    public bool Loop
    {
        get => audioSource.loop;
        set => audioSource.loop = value;
    }
    public AudioClip Clip
    {
        get => audioSource.clip;
        set => audioSource.clip = value;
    }
    public bool IsPlaying => audioSource.isPlaying;

    #endregion

    private void Awake()
    {
        smartSound3DController = SmartSound3DController.Singleton;
        smartSound3DController.AddSound(this);

        audioSource = GetComponent<AudioSource>();
        if (audioSource.spatialBlend != 0)
        {
            audioSource.spatialBlend = 0;
            Debug.LogWarning("SmartSound3D working only with 2D AudioSource (Spatial Blend = 0)");
        }
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
        smartSound3DController.RemoveSound(this);
    }

    public void UpdateVolume()
    {
        float d1Sqr = player1 ? (transform.position - player1.transform.position).sqrMagnitude : Mathf.Infinity;
        float d2Sqr = player2 ? (transform.position - player2.transform.position).sqrMagnitude : Mathf.Infinity;
        float nearestSqr = Mathf.Min(d1Sqr, d2Sqr);

        audioSource.volume = Mathf.Clamp01(1f - nearestSqr / (maxHearingDistance * maxHearingDistance));
    }

    public void Play()
    {
        UpdateVolume();
        audioSource.Play();
    }

    public void Stop() => audioSource.Stop();
}
