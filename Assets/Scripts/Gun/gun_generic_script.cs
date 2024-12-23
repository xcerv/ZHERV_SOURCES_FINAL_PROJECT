using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class gun_generic_script : MonoBehaviour
{
    public GameObject Player;
    

    [SerializeField] UnityEvent OnGunShoot;
    public float g_fire_cooldown = 0.5f;
    public bool semi = true;
    public bool gun_active = false;

    private float g_current_cooldown;
    
    // Start is called before the first frame update
    void Start()
    {
        g_current_cooldown = g_fire_cooldown;
    }


    // Button press detection
    void Update()
    {
        if (!gun_active)
            return;

        if(semi)
        {
            if(Input.GetMouseButton(0))
            {
                if(g_current_cooldown <= 0)
                {
                    OnGunShoot?.Invoke();
                    g_current_cooldown = g_fire_cooldown;
                }
            }
        }
        else
        {
            if(Input.GetMouseButton(0))
            {
                if(g_current_cooldown <= 0)
                {
                    OnGunShoot?.Invoke();
                    g_current_cooldown = g_fire_cooldown;
                }
            }
        }

        //Debug.Log(g_current_cooldown);
        g_current_cooldown -= Time.deltaTime;

    } // Update
}
