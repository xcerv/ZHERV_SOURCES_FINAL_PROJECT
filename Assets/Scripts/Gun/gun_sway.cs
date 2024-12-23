using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun_sway : MonoBehaviour
{
    public float sway_smooth = 20f;
    public float sway_multiplier = 2.0f;

    private Quaternion lastRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        float MouseX = Input.GetAxisRaw("Mouse X") * sway_multiplier;
        float MouseY = Input.GetAxisRaw("Mouse Y") * sway_multiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-MouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(MouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;
        //Quaternion targetRotation = Quaternion.Inverse(lastRotation) * transform.rotation; lastRotation = transform.rotation;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * sway_smooth);
    }
}
