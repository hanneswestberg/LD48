using UnityEngine;
using System.Collections;

public class ParentSpringJointRotation : MonoBehaviour
{
    private SpringJoint2D joint;
    private Quaternion originalRotation;
    private Quaternion inverseParentRotation;

    // Initialization
    void Start()
    {
        joint = gameObject.GetComponent<SpringJoint2D>();
        originalRotation = joint.transform.rotation;
        inverseParentRotation = Quaternion.Inverse(joint.connectedBody.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion parentDeltaRotation = (joint.connectedBody.transform.rotation * inverseParentRotation);
        joint.transform.rotation = (parentDeltaRotation * originalRotation);
    }
}