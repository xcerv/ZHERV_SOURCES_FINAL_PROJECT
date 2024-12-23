using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class entity_generic : MonoBehaviour
{
    [SerializeField] private float initial_hp;
    private float health;
    [SerializeField] UnityEvent OnEntityDeath;
    [SerializeField] UnityEvent OnEntityHurt;
    private bool death = false;

    public float Health
    {
        get
        {
            Debug.Log("My health is: " + health);
            return health;
        }

        set
        {
            float lastHp = health;
            health = value;
            if(health <= 0f && !death)
            {
                death = true;
                OnEntityDeath?.Invoke();
            }
            else if (health-lastHp <= 0 )
            {
                OnEntityHurt?.Invoke();
            }
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        Health = initial_hp;
    }

    public void reduceHealth(float amount)
    {
        Health -= amount;
    }
}
