using System.Collections.Generic;
using UnityEngine;
using static ViewPortController;

public class SmartSound3DController : MonoBehaviour
{
    private List<SmartSound3D> sounds;

    public static SmartSound3DController Singleton { get; private set; }
    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }

    private void Awake()
    {
        Singleton = this;
        sounds = new();
    }

    private void Update()
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
