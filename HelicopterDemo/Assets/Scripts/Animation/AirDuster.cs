using UnityEngine;

public class AirDuster : MonoBehaviour
{
    [SerializeField] private float particleSpeed = 15f;

    private ParticleSystem dust;

    public float normRotorSpeed { get; set; } //0...1 (1 - max)
    public float normAltitiude { get; set; } //0...1 (1 - max)

    void Start()
    {
        dust = GetComponent<ParticleSystem>();
        normRotorSpeed = 0f;
        normAltitiude = 0f;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        if (!dust.isPlaying) dust.Play();

        if (normRotorSpeed < 1f)
            dust.startSpeed = normRotorSpeed * particleSpeed;
        else if (normAltitiude < 1f)
        {
            float altDustCoef = 1f - normAltitiude;
            if (altDustCoef < 0f) altDustCoef = 0f;
            dust.startSpeed = altDustCoef * particleSpeed;
        }
        else
            dust.Stop();
    }
}
