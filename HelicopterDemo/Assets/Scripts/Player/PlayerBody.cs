using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField] private GameObject bombHolder;
    [SerializeField] private GameObject flagHolder;
    [SerializeField] private GameObject keyHolder;

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
            Bomb bomb = ItemForTake.GetComponent<Bomb>();
            Flag flag = ItemForTake.GetComponent<Flag>();
            Key key = ItemForTake.GetComponent<Key>();
            if (bomb)
            {
                Item.transform.position = bombHolder.transform.position;
                Item.transform.rotation = bombHolder.transform.rotation;
            }
            else if (flag)
            {
                Item.transform.position = flagHolder.transform.position;
                Item.transform.rotation = flagHolder.transform.rotation;
            }
            else if (key)
            {
                Item.transform.position = keyHolder.transform.position;
                Item.transform.rotation = keyHolder.transform.rotation;
            }
        }
    }

    public void Take()
    {
        Bomb bomb = ItemForTake.GetComponent<Bomb>();
        Flag flag = ItemForTake.GetComponent<Flag>();
        Key key = ItemForTake.GetComponent<Key>();
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
        else if (flag && flagHolder)
        {
            Item = ItemForTake;
            Item.transform.SetParent(transform);
            Item.transform.position = flagHolder.transform.position;
            Item.transform.rotation = flagHolder.transform.rotation;
            Item.transform.localScale = new Vector3(Item.transform.localScale.x, -Item.transform.localScale.y, Item.transform.localScale.z);
            Item.SetGravity(false);
            Item.SetTrigger(false);
        }
        else if (key && keyHolder)
        {
            Item = ItemForTake;
            Item.transform.SetParent(transform);
            Item.transform.position = keyHolder.transform.position;
            Item.transform.rotation = keyHolder.transform.rotation;
            Item.SetGravity(false);
            Item.SetTrigger(false);
        }
    }

    public void Drop()
    {
        if (Item)
        {
            if (Item.GetComponent<Key>())
            {
                //todo remove tags
                GameObject vault = GameObject.FindGameObjectWithTag("EnemyFlagVault");
                if (vault)
                {
                    float dist = Vector3.Magnitude(vault.transform.position - transform.position);
                    if (dist < 10)
                    {
                        vault.GetComponent<FlagVault>().SetKey(Types.KeyType.Purple);
                        Destroy(Item.gameObject);
                    }
                    else
                        LocalDrop();
                }
            }
            else if (Item.GetComponent<Bomb>())
            {
                var bomb = Item.GetComponent<Bomb>();
                bomb.PlaySound();
                LocalDrop();
            }
            else
                LocalDrop();
        }
    }

    private void LocalDrop()
    {
        Item.transform.SetParent(null);
        Item.SetGravity(true);
        Item = null;
    }
}
