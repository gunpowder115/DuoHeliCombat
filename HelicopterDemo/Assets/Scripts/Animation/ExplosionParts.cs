using System.Collections.Generic;
using UnityEngine;

public class ExplosionParts : MonoBehaviour
{
    [SerializeField] private float forceLimit = 100f;

    private List<Rigidbody> parts;

    private void Start()
    {
        parts = new List<Rigidbody>();
        parts.AddRange(GetComponentsInChildren<Rigidbody>());

        foreach (var part in parts)
        {
            part.gameObject.transform.parent = null;
            part.gameObject.AddComponent(typeof(FadingOut));
            Vector3 force = new Vector3(0f, Random.Range(forceLimit / 2f, forceLimit), 0f);
            part.AddForce(force);
        }
    }
}