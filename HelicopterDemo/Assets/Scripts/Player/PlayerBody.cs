using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    public Bomb BombForTake { get; set; }

    public void Take(GameObject item)
    {
        item.transform.SetParent(transform);
    }
}
