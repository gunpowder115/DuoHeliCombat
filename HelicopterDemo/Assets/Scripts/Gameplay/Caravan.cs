using System.Collections.Generic;
using UnityEngine;

public class Caravan : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float caravanDist = 5f;

    private List<GameObject> escort;
    private List<Vector3> relEscortPositions, escortPositions;

    public Vector3 Speed => transform.forward * speed;

    private void Awake()
    {
        escort = new List<GameObject>();

        relEscortPositions = new List<Vector3>();
        relEscortPositions.Add((transform.forward + transform.right) * caravanDist);
        relEscortPositions.Add((transform.forward - transform.right) * caravanDist);
        relEscortPositions.Add((-transform.forward + transform.right) * caravanDist);
        relEscortPositions.Add((-transform.forward - transform.right) * caravanDist);

        escortPositions = new List<Vector3>();
        escortPositions.Add(relEscortPositions[0]);
        escortPositions.Add(relEscortPositions[1]);
        escortPositions.Add(relEscortPositions[2]);
        escortPositions.Add(relEscortPositions[3]);
    }

    private void Update()
    {
        transform.Translate(Speed * Time.deltaTime, Space.World);

        for (int i = 0; i < escortPositions.Count; i++)
            escortPositions[i] = transform.position + relEscortPositions[i];

        foreach (var item in escort)
        {
            if (!item)
                escort.Remove(item);
        }
    }

    public void AddEscortItem(GameObject item)
    {
        if (!escort.Contains(item))
            escort.Add(item);
    }

    public Vector3 GetEscortItemTargetPosition(GameObject item)
    {
        Vector3 result = escortPositions[0];
        for (int i = 0; i < escort.Count; i++)
        {
            if (item == escort[i])
                return escortPositions[i];
        }
        return result;
    }
}
