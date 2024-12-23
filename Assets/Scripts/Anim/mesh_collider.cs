using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mesh_collider : MonoBehaviour
{

    public SkinnedMeshRenderer meshRenderer;
    public MeshCollider colliderr;
   
    private Mesh colliderMesh;
    private bool shouldUpdate = true;

    public void UpdateCollider() {
        colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        colliderr.sharedMesh = null;
        colliderr.sharedMesh = colliderMesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateCollider();
    }

    // Update is called once per frame
    void UpdateMesh()
    {
        if(shouldUpdate){
            meshRenderer.BakeMesh(colliderMesh);
            colliderr.sharedMesh = colliderMesh;        
        }
    }

    public void stopUpdate()
    {
        shouldUpdate = false;
    }

    public void startUpdate()
    {
        shouldUpdate = true;
    }

    public void disableCollision()
    {
        colliderr.enabled = false;
    }

    public void enableCollision()
    {
        colliderr.enabled = false;
    }
}
