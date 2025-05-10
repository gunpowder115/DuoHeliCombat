using System.Collections.Generic;
using UnityEngine;

public class Tether : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private int segmentCount = 20;
    [SerializeField] private float segmentLength = 0.5f;

    private List<GameObject> segments = new List<GameObject>();

    void Start()
    {
        Vector3 ropeDirection = (endPoint.position - startPoint.position).normalized;
        Vector3 spacing = ropeDirection * segmentLength;

        Vector3 currentPos = startPoint.position;

        GameObject previous = null;

        for (int i = 0; i < segmentCount; i++)
        {
            Quaternion rotation = Quaternion.LookRotation(ropeDirection) * Quaternion.Euler(90, 0, 0);
            GameObject segment = Instantiate(segmentPrefab, currentPos, Quaternion.identity, transform);
            segments.Add(segment);

            Rigidbody rb = segment.GetComponent<Rigidbody>();

            if (i == 0)
            {
                ConfigurableJoint joint = segment.AddComponent<ConfigurableJoint>();
                joint.connectedBody = startPoint.GetComponent<Rigidbody>();
                ConfigureJoint(joint);
            }
            else
            {
                ConfigurableJoint joint = segment.AddComponent<ConfigurableJoint>();
                joint.connectedBody = previous.GetComponent<Rigidbody>();
                ConfigureJoint(joint);
            }

            currentPos += spacing;
            previous = segment;
        }

        ConfigurableJoint endJoint = segments[segments.Count - 1].AddComponent<ConfigurableJoint>();
        endJoint.connectedBody = endPoint.GetComponent<Rigidbody>();
        ConfigureJoint(endJoint);

        for (int i = 0; i < segments.Count - 1; i++)
        {
            var visual = segments[i].GetComponent<TetherSegmentVisual>();
            visual.Target = segments[i + 1].transform;
        }
        segments[segments.Count - 1].GetComponent<TetherSegmentVisual>().Target = endPoint;

        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].GetComponent<TetherSegmentVisual>().IsVisible = i < segments.Count - 1;
        }
    }

    void ConfigureJoint(ConfigurableJoint joint)
    {
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = Vector3.zero;

        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit linearLimit = new SoftJointLimit();
        linearLimit.limit = segmentLength;
        joint.linearLimit = linearLimit;

        JointDrive drive = new JointDrive();
        drive.positionSpring = 0;
        drive.positionDamper = 50;
        drive.maximumForce = Mathf.Infinity;

        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;
    }
}