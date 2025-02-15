using System.Collections.Generic;
using UnityEngine;

public class Caravan : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private List<GameObject> escort;

    private void Awake()
    {
        escort = new List<GameObject>();
    }

    private void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    public void AddEscortItem(GameObject item)
    {
        if (!escort.Contains(item))
            escort.Add(item);
    }
}
