using UnityEngine;

public class TakeoffProcess : MonoBehaviour
{
    [SerializeField] private float climbSpeed = 0.5f;
    [SerializeField] private float takeoffHeight = 10f;
    [SerializeField] private SimpleRotor clearRotor;
    [SerializeField] private SimpleRotor blurryRotor;
    [SerializeField] private Camera playerCamera;
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
        switch(TakeoffPhase)
        {
            case TakeoffPhases.RotorAcceleration:
                ClimbSpeed = 0f;
                if (clearRotor.ReadyToTakeoff)
                    TakeoffPhase = TakeoffPhases.Climbing;
                return false;
            case TakeoffPhases.Climbing:
                ClimbSpeed = climbSpeed;
                if (clearRotor.transform.position.y >= playerCamera.transform.position.y)
                    BladesSwipe();
                return player.transform.position.y > takeoffHeight;
            default: return false;
        }
    }

    private void BladesSwipe()
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
