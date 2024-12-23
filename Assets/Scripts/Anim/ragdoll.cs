using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ragdoll : MonoBehaviour
{
    private Rigidbody[] ragdollRigidbodies;
    private mesh_collider colliderScript;
    private Animator animatorParent;

    // Start is called before the first frame update
    void Awake()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }

    public void DisableRagdoll()
    {
        foreach (var rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
            //rigidbody.detectCollisions = false;
        }
    }

    public void EnableRagdoll()
    {

        foreach (var rigidbody in ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
            //rigidbody.detectCollisions = true;
        }
    }
}
