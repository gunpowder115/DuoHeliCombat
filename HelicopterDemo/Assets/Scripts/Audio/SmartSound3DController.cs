using System.Collections.Generic;
using static ViewPortController;

public class SmartSound3DController
{
    private List<SmartSound3D> sounds;

    private static SmartSound3DController singleton;

    public static SmartSound3DController Singleton
    {
        get
        {
            if (singleton == null)
                singleton = new SmartSound3DController();
            return singleton;
        }

    }
    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }

    private SmartSound3DController()
    {
        sounds = new();
    }

    public void UpdateAllVolumes()
    {
        foreach (var sound in sounds)
            sound.UpdateVolume();
    }

    public void AddPlayer(Player player, Players playerNumber)
    {
        if (playerNumber == Players.Player1)
            Player1 = player;
        else
            Player2 = player;
    }

    public void AddSound(SmartSound3D sound) => sounds.Add(sound);

    public void RemoveSound(SmartSound3D sound) => sounds.Remove(sound);
}
