using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField] private GameObject bombHolder;

    public PickableUp ItemForTake { get; set; }
    public PickableUp Item { get; private set; }


    private void Start()
    {
        Item = null;
    }

    private void Update()
    {
        if (Item)
        {
            Item.transform.position = bombHolder.transform.position;
            Item.transform.rotation = bombHolder.transform.rotation;
        }
    }

    public void Take()
    {
        Bomb bomb = ItemForTake.GetComponent<Bomb>();
        if (bomb && bombHolder)
        {
            Item = ItemForTake;
            Item.transform.SetParent(transform);
            Item.transform.position = bombHolder.transform.position;
            Item.transform.rotation = bombHolder.transform.rotation;
            Item.SetGravity(false);
            Item.SetTrigger(false);
            bomb.IsActivated = true;
        }
    }

    public void Drop()
    {
        if (Item)
        {
            Item.transform.SetParent(null);
            Item.SetGravity(true);
            Item = null;
        }
    }
}
