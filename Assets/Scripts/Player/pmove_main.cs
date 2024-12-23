using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PMoveUtil;
using Unity.Mathematics;
using UnityEditor;
using System;
using UnityEngine.Events;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : PortalTraveller
{
    // Custom globals
    public movevars_t movevars;
    public playermove_t pmove;
    public _cmd cmd;
    public _options opt;

    // Conditions
    private int onGround = -1;
    private int waterLevel;
    private int waterType;

    
    private const int STEPSIZE = 18;
    private const float STOP_EPSILON = 0.1f;

    // Entities
    private CharacterController characterController;
    private Camera playerCamera;
    private Transform playerView;
    private GameObject player;

    private Vector3 player_mins = new Vector3();
    private Vector3 player_maxs = new Vector3();

    // Audio
    public sound_controller_generic soundController;
    public List<AudioClip> snd_jump;

    // Flags
    public bool moveDisabled = false;
    public bool moveSlowed = false;

    private float trFrac(Vector3 start, Vector3 endpos, RaycastHit hit){
        return (endpos-start).magnitude / hit.distance;
    }

    // START AND UPDATE

    void Start()
    {
        soundController = GetComponent<sound_controller_generic>();

        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        if (playerCamera != null)
            playerView = playerCamera.gameObject.transform;

        pmove.velocity = characterController.velocity;
        pmove.origin = characterController.transform.position;
        pmove.angles = characterController.transform.eulerAngles;

        player_mins = characterController.bounds.min;
        player_maxs = characterController.bounds.max;

        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + opt.opt_cam_offset,
            transform.position.z);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if(!moveDisabled)
            determine_cmd();
        else{
            cmd.fmove = 0;
            cmd.smove = 0;
            cmd.m_rotX = 0;
            cmd.m_rotY = 0;
        }

        determine_condition();
        

        this.transform.rotation = Quaternion.Euler(0, cmd.m_rotY, 0);
        playerView.rotation = Quaternion.Euler(cmd.m_rotX, cmd.m_rotY, 0);
 
        if(onGround != -1){
            pm_groundmove();
        }
        else{
            pm_airmove();
        }

        //TeleportCheck();

        // Debug.Log(pmove.velocity);
        if(moveSlowed)
            pmove.velocity *= 0.96f;

        characterController.Move(pmove.velocity * Time.deltaTime);

        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + opt.opt_cam_offset,
            transform.position.z);
    }


    // MOVEMENT FUNCTIONS

    void pm_airmove()
    {
        float accel;

        Vector3 wishdir = new Vector3(cmd.smove, 0, cmd.fmove);
        wishdir = transform.TransformDirection(wishdir);
        float wishspeed = wishdir.magnitude * movevars.movespeed;
        wishdir.Normalize();

        // Clamp speed
        if(wishspeed > movevars.maxspeed){
            wishspeed = movevars.maxspeed;
            // May add specific speed for strafing
        }

        if(Vector3.Dot(pmove.velocity, wishdir) < 0){
            accel = movevars.airdeaccelerate; // Can change for deaccel
        }
        else{
            accel = movevars.airaccelerate;
        }

        pm_accelerate(wishdir, wishspeed, accel);
        if(movevars.aircontrol > 0){
            pm_aircontrol(wishdir, wishspeed);
        }

        pmove.velocity.y -= movevars.entgravity * movevars.gravity * Time.deltaTime; 
    }

    void pm_aircontrol(Vector3 wishdir, float wishspeed){
        float zspeed;
        float speed;
        float dot;
        float k = 32;

        if(Mathf.Abs(cmd.fmove) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
            return;
        
        zspeed = pmove.velocity.y;
        pmove.velocity.y = 0;

        speed = pmove.velocity.magnitude;
        pmove.velocity.Normalize();

        dot = Vector3.Dot(pmove.velocity, wishdir);
        k *= movevars.aircontrol * dot * dot * Time.deltaTime;

        if(dot > 0){
            pmove.velocity = pmove.velocity * speed + wishdir * k;
            pmove.velocity.Normalize();

        } 

        pmove.velocity.x *= speed;
        pmove.velocity.y = zspeed; // Note this line
        pmove.velocity.z *= speed;
    }

    void pm_accelerate(Vector3 wishdir, float wishspeed, float accel){
        float addspeed;
        float accelspeed;
        float currentspeed;

        /*
        if(pmove.dead)
            return;
        if ( pmove.waterjumptime != 0 )
            return;
        */

        currentspeed = Vector3.Dot(pmove.velocity, wishdir);
        addspeed = wishspeed - currentspeed;
        
        if (addspeed <= 0)
            return;
        
        accelspeed = accel * Time.deltaTime * wishspeed;

        if(accelspeed > addspeed)
            accelspeed = addspeed;
        
        pmove.velocity.x += accelspeed * wishdir.x;
        pmove.velocity.z += accelspeed * wishdir.z;
    }

    void pm_friction(float t){
        
        Vector3 vec = pmove.velocity;
        float	speed, newspeed, control;
        float	friction;
        float	drop;
        Vector3	start;
        Vector3 stop;
        bool		trace;
        RaycastHit hit;
        
        //if ( pmove.waterjumptime != 0 )
          //  return;
        
        vec.y = 0;
        speed = vec.magnitude;
        friction = movevars.friction;
        drop = 0;


        if (speed < 0.1f)
        {
            pmove.velocity.x = 0;
            pmove.velocity.z = 0;
            return;
        }


        // if the leading edge is over a dropoff, increase friction
        /*
        if (onGround != -1) {
            start = stop = pmove.origin + pmove.velocity/speed*16;
            start.y = pmove.origin.y ; //+ player_mins.y;
            stop.y = start.y - 34;

            trace = Physics.Linecast (start, stop, out hit );

            if (trace && trFrac(start, stop, hit) >= 1) {
                friction *= 2;
            }
        }
        */


        //if (waterLevel >= 2) // apply water friction
        //    drop += speed*movevars.waterfriction*waterLevel*Time.deltaTime;
        if (onGround != -1) // apply ground friction
        {
            control = speed < movevars.deaccelerate ? movevars.deaccelerate : speed;
            drop = control * friction * Time.deltaTime * t;
        }


    // scale the velocity
        newspeed = speed - drop;
        if (newspeed < 0)
            newspeed = 0;
        if(speed > 0)
            newspeed /= speed;

        pmove.velocity.x *= newspeed;
        pmove.velocity.z *= newspeed;
    }

    void determine_condition(){
        if (characterController.isGrounded){
            onGround = 1;
        }
        else{
            onGround = -1;
        }
    }

    void determine_cmd(){
        cmd.fmove = Input.GetAxisRaw("Vertical");
        cmd.smove = Input.GetAxisRaw("Horizontal") * movevars.strafe_boost;

        cmd.m_rotX = math.clamp(cmd.m_rotX - Input.GetAxisRaw("Mouse Y") * opt.opt_m_sensX * 0.08f, -90, 90);
        cmd.m_rotY = (cmd.m_rotY + Input.GetAxisRaw("Mouse X") * opt.opt_m_sensY * 0.08f) % 360 ;

        // Jumping
        if(Input.GetButtonDown("Jump") && !cmd.wishjump){
            cmd.wishjump = true;
        }
        else if(Input.GetButtonUp("Jump")){
            cmd.wishjump = false;
        }
    }

    void pm_groundmove(){
        if(!cmd.wishjump)
            pm_friction(1.0f);
        else{
            pm_friction(0);
        }

        Vector3 wishdir = new Vector3(cmd.smove, 0, cmd.fmove);
        wishdir = transform.TransformDirection(wishdir).normalized;
        float wishspeed = wishdir.magnitude * movevars.movespeed;

        // Clamp speed
        if(wishspeed > movevars.maxspeed){
            // wishvel *= (movevars.maxspeed/wishspeed);
            wishspeed = movevars.maxspeed;
        }

        pmove.velocity.y = 0;
        wishdir.y = 0;
        pm_accelerate(wishdir, wishspeed, movevars.accelerate);
        pmove.velocity.y -= movevars.entgravity * movevars.gravity * Time.deltaTime; 

        if(cmd.wishjump)
        {
            soundController.PlaySound(snd_jump[UnityEngine.Random.Range(0, snd_jump.Count)]); // Play jump sound

            pmove.velocity.y += movevars.jumpspeed ;
            cmd.wishjump = false;
        }
    }

    public void slowMove(bool slow)
    {
        if(slow)
            moveSlowed = true;
        else
            moveSlowed = false;
    }

    [SerializeField]
    UnityEvent onPlayerTeleport;

    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        var duo = fromPortal.parent.gameObject.GetComponent<PortalDuo>();
        if(!duo.portal_crossed)
        {   
            duo.portal_crossed = true;
            onPlayerTeleport.Invoke();
        }


        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle (cmd.m_rotY, eulerRot.y);
        cmd.m_rotY += delta;

        transform.eulerAngles = Vector3.up * cmd.m_rotY;
        
        pmove.velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (pmove.velocity));
        Physics.SyncTransforms ();
    }
}