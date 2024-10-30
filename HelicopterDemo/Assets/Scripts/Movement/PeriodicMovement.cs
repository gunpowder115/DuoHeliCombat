using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicMovement : MonoBehaviour
{
    [SerializeField] private float delta = 5f;
    [SerializeField] private float speed = 5f;

    private Vector3 currPos, upPos, downPos, tgtPos;

    // Start is called before the first frame update
    void Start()
    {
        currPos = transform.position;
        upPos = currPos + new Vector3(0f, delta, 0f);
        downPos = currPos - new Vector3(0f, delta, 0f);
        tgtPos = upPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Magnitude(currPos - upPos) < 0.1f) tgtPos = downPos; 
        if (Vector3.Magnitude(currPos - downPos) < 0.1f) tgtPos = upPos;
        currPos = Vector3.Lerp(currPos, tgtPos, speed * Time.deltaTime);
        transform.position = currPos;
    }
}
