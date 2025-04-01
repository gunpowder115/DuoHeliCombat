using UnityEngine;

public class TakeoffProcess : MonoBehaviour
{
    [SerializeField] private float climbSpeed = 0.5f;
    [SerializeField] private float takeoffHeight = 10f;
    [SerializeField] private SimpleRotor clearRotor;
    [SerializeField] private SimpleRotor blurryRotor;
    [SerializeField] private CameraMovement playerCamera;
    [SerializeField] private AudioSource audioSource;

    private Player player;

    public float ClimbSpeed { get; private set; }
    public TakeoffPhases TakeoffPhase { get; private set; }

    private void Start()
    {
        clearRotor.gameObject.SetActive(true);
        blurryRotor.gameObject.SetActive(false);

        TakeoffPhase = TakeoffPhases.RotorAcceleration;

        player = GetComponent<Player>();
    }

    public bool Takeoff()
    {
        if (clearRotor.isActiveAndEnabled)
            audioSource.pitch = clearRotor.RotSpeedCoef;
        else
            audioSource.pitch = 1f;

        switch (TakeoffPhase)
        {
            case TakeoffPhases.RotorAcceleration:
                ClimbSpeed = 0f;
                if (clearRotor.ReadyToTakeoff)
                {
                    TakeoffPhase = TakeoffPhases.Climbing;
                    playerCamera.MoveCamera = true;
                }
                playerCamera.CameraSpeedInTakeoff = 0f;
                return false;
            case TakeoffPhases.Climbing:
                ClimbSpeed = climbSpeed;
                bool result = player.transform.position.y > takeoffHeight;
                playerCamera.CameraSpeedInTakeoff = 0.7f;
                if (result)
                {
                    BladesSwipe();
                    playerCamera.CameraInTakeoff = false;
                }
                return result;
            default: return false;
        }
    }

    public void BladesSwipe()
    {
        clearRotor.gameObject.SetActive(false);
        blurryRotor.gameObject.SetActive(true);
    }

    public enum TakeoffPhases
    {
        RotorAcceleration,
        Climbing
    }
}
