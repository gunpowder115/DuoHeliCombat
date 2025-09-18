using UnityEngine;

public class SoundVolumeController : MonoBehaviour
{
    private SmartSound3DController soundController;

    private void Awake()
    {
        soundController = SmartSound3DController.Singleton;
    }

    private void Update()
    {
        soundController.UpdateAllVolumes();
    }
}
