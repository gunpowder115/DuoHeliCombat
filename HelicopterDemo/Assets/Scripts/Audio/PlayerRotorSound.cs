using UnityEngine;

public class PlayerRotorSound : MonoBehaviour
{
    private AudioSource audioSource;
    private UnitController unitController;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        unitController = UnitController.Singleton;
    }

    private void Update()
    {
        foreach (var player in unitController.Players)
        {
            if ((player as Player).IsAlive)
            {
                audioSource.volume = 1f;
                return;
            }            
        }
        audioSource.volume = 0f;
    }
}
