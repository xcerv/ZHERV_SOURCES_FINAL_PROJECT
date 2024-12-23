using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject PortalStart;
    public GameObject PortalEnd;

    public GameObject Entities;

    void Awake()
    {
        //Entities.SetActive(false);
    }

    public void SpawnPortals(Portal portal_start, Portal portal_end)
    {
        if(portal_start)
        {
            portal_start.transform.position = PortalStart.transform.position;
            portal_start.transform.rotation = PortalStart.transform.rotation;
        }

        if(portal_end)
        {
            portal_end.transform.position = PortalEnd.transform.position;
            portal_end.transform.rotation = PortalEnd.transform.rotation;
        }
    }


    private GameObject currentInstance;
    public void SpawnEntities()
    {
        if(Entities){
            //Debug.Log("SPAWN");
            currentInstance = Instantiate(Entities, transform, true);
            currentInstance.SetActive(true);
        }  
    }
    
    public void DestroyEntities()
    {
        if(currentInstance)
        {
            //Debug.Log("DESPAWN");
            Destroy(currentInstance);
        }
    }
}
