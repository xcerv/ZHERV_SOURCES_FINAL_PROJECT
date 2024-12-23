using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{

    public bool triggerOnce = true;
    public float repeatTime = -1.0f; // -1 == disabled
    
    private float lastRepeat = 0;
    private bool isColliding = false;

    [SerializeField]
    UnityEvent onTriggerEnter;

    [SerializeField]
    UnityEvent onTriggerExit;

    void OnTriggerEnter(Collider other)
    {
        if(triggerOnce)
        {
            onTriggerEnter?.Invoke();
            Destroy(gameObject);
        }
        else
        {
            isColliding = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        isColliding = false;
        onTriggerExit?.Invoke();
    }

    public void DestroyEnt(GameObject obj)
    {
        Destroy(obj);
    }

    void Update()
    {
        if(isColliding && repeatTime >= 0){
            if(lastRepeat < 0)
            {
                onTriggerEnter?.Invoke();
                lastRepeat = repeatTime;
            }
        }

        lastRepeat -= Time.deltaTime;
    }

}
