using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class entity_player : MonoBehaviour
{
    public TMP_Text mainUI_Health;
    private sound_controller_generic soundControllerHurt;
    private entity_generic genericHealthManager;
    
    public GameManager gameManager;

    public List<AudioClip> hurt_clips;

    // Start is called before the first frame update
    void Start()
    {
        soundControllerHurt = GetComponentInParent<sound_controller_generic>();
        genericHealthManager = GetComponent<entity_generic>();
    }

    void Update()
    {

    }

    public void Hurt()
    {
        if(playerDead)
            return;

        // Hurt sounds
        soundControllerHurt.PlaySoundList(hurt_clips);

        // UI
        mainUI_Health.text = genericHealthManager.Health.ToString();
    }

    private bool playerDead = false;
    public void Death()
    {
        if (playerDead)
            return;

        playerDead = true;
        mainUI_Health.text = "DEAD";

        var movement = GetComponentInParent<PlayerMovement>();
        if(movement)
            movement.moveDisabled = true;
        
        Camera.main.transform.rotation *= Quaternion.Euler(0,0,90);

        if(gameManager)
            gameManager.LoseGame();
        //Destroy(gameObject);
    }
}
