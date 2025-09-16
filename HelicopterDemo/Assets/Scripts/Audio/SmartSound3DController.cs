using System.Collections.Generic;
using UnityEngine;
using static ViewPortController;

public class SmartSound3DController : MonoBehaviour
{
    private Player player1;
    private Player player2;
    private List<SmartSound3D> sounds;

    public static SmartSound3DController Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;
        sounds = new();
    }

    private void Update()
    {
        foreach (var sound in sounds)
            sound.UpdateVolume(player1, player2);
    }

    public void AddPlayer(Player player, Players playerNumber)
    {
        if (playerNumber == Players.Player1)
            player1 = player;
        else
            player2 = player;
    }

    public void AddSound(SmartSound3D sound) => sounds.Add(sound);
}
