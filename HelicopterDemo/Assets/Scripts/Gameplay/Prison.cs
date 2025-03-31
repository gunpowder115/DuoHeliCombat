using System.Collections.Generic;
using UnityEngine;

public class Prison : MonoBehaviour
{
    [SerializeField] private float radius = 2.5f;
    [SerializeField] private GameObject man;

    private const int MEN_COUNT = 3;

    private List<ManAnimator> manAnimators;

    private void Start()
    {
        manAnimators = new List<ManAnimator>();
        for (int i = 0; i < MEN_COUNT; i++)
        {
            var animator = Instantiate(man).GetComponent<ManAnimator>();
            manAnimators.Add(animator);
        }
        manAnimators[0].SetBorders(transform.position, 0f, -radius, 0f, -radius);
        manAnimators[1].SetBorders(transform.position, 0f, radius, 0f, -radius);
        manAnimators[2].SetBorders(transform.position, -radius, radius, 0f, radius);
    }

    public void MoveToPlayer(Player player)
    {
        foreach (var man in manAnimators)
            man.SetArrivingHelicopter(player.gameObject);
    }

    public void RemoveMen()
    {
        foreach (var man in manAnimators)
            man.DestroyMan();
    }
}
