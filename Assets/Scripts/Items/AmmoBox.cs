using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int ammo_gain = 24;

    private void OnTriggerEnter(Collider other)
    {


        var shotgun = other.transform.root.gameObject.GetComponentInChildren<gun_shoot_shotgun>(); 

        if(shotgun)
        {
            //shotgun.ammo += ammo_gain;
            shotgun.Reload(ammo_gain);
            Destroy(transform.parent.gameObject);
        }
    }
}
