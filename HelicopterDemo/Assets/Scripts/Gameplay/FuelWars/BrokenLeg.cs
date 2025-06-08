using System.Collections.Generic;
using UnityEngine;

public class BrokenLeg : MonoBehaviour
{
    private List<Rigidbody> rigidbodies;
    
    private void Awake()
    {
        rigidbodies = new List<Rigidbody>();
        rigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());

        rigidbodies[0].AddForce(new Vector3(0f, 0f, 50f), ForceMode.VelocityChange);
        rigidbodies[1].AddForce(new Vector3(0f, 0f, -50f), ForceMode.VelocityChange);
    }
}
