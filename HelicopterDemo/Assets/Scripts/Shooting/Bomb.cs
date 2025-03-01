using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerBody player = other.gameObject.GetComponent<PlayerBody>();
        if (player)
        {
            player.BombForTake = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerBody player = other.gameObject.GetComponent<PlayerBody>();
        if (player)
        {
            player.BombForTake = null;
        }
    }
}