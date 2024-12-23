using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class entity_orb : MonoBehaviour
{

    private bool orbDead = false;
    private sound_controller_generic soundControllerOrb; 
    public Light orbLight; 

    private void Awake()
    {
        soundControllerOrb = GetComponent<sound_controller_generic>();
    }

    // Start is called before the first frame update
    public void OrbDeath()
    {
        if(orbDead)
            return;

        orbDead = true;
        orbLight.enabled = false; 

        Orb_Audio();

        GetComponent<SphereCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

        Invoke(nameof(destroySelf), 1f);
    }

    // Orb sounds
    public List<AudioClip> orb_clips;
    public void Orb_Audio()
    {
        soundControllerOrb.PlaySoundList(orb_clips);
    }

    void destroySelf()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}
