using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class entity_npc : MonoBehaviour
{
    // Entities
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask layerGround, layerPlayer;
    public Transform muzzle;

    // AI Behaviour
    public float attack_cooldown = 2f;
    public bool attacked = false;
    public float attack_dmg = 5f;

    public float sightRange = 15f, attackRange = 1.5f;
    public bool playerInSight = false, playerInAttack = false;

    // Flags
    private bool dead = false;
    private bool idle_clip_played = false;

    // Audio
    public sound_controller_generic soundController;
    public List<AudioClip> npc_hurt_clips;
    public List<AudioClip> npc_death_clips;
    public List<AudioClip> npc_attack_clips;
    public List<AudioClip> npc_idle_clips;

    // Anim
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        soundController = GetComponent<sound_controller_generic>();
    }

    public void Idle()
    {
        animator.SetBool("IsChasing", false);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsIdle", true);

        if(!idle_clip_played)
        {
            idle_clip_played = true;
            Invoke(nameof(Audio_Idle), Random.Range(6, 12.0f));
        }
    }

    public void ChasePlayer()
    {
        animator.SetBool("IsChasing", true);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsIdle", false);

        agent.SetDestination(player.position);
    }

    public void AttackPlayer()
    {
        animator.SetBool("IsChasing", false);
        animator.SetBool("IsAttacking", true);
        animator.SetBool("IsIdle", false);

         agent.SetDestination(transform.position);

         //transform.LookAt(player);

         if(!attacked)
         {
            // Attack here
            // animator.SetBool("IsAttacking", true);
            Ray gunray = new Ray(muzzle.position, Vector3.Lerp(muzzle.forward, (player.position-muzzle.position).normalized, 0.5f));
            if( Physics.Raycast(gunray, out RaycastHit hit, attackRange, layerPlayer) )
            {
                if(hit.collider.gameObject.TryGetComponent(out entity_generic plr_enemy)){
                    plr_enemy.Health -= attack_dmg;
                }
            }
    

            attacked = true;
            Invoke(nameof(ResetAttack), attack_cooldown);
         }
    }

    private void ResetAttack()
    {
        attacked = false;
        //animator.SetBool("IsAttacking", false);
    }

    private Rigidbody mRigidBody;
    private GameObject plr;
    public float npc_speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        if (dead)
            return;

        playerInSight = Physics.CheckSphere(transform.position, sightRange, layerPlayer);
        playerInAttack = Physics.CheckSphere(transform.position, attackRange, layerPlayer);

        if( !playerInSight && !playerInAttack )
            Idle();
        if (playerInSight && !playerInAttack)
            ChasePlayer();
        if(playerInSight && playerInAttack)
            AttackPlayer();
    
    }

    public void Death()
    {
        dead = true;

        // Disable mesh collisions
        var colliderScript = GetComponent<mesh_collider>(); 
        if(colliderScript)
        {
            colliderScript.stopUpdate();
            colliderScript.disableCollision();
        }

        // Disable mesh collider if it exists
        var mesh_collider = transform.GetComponent<MeshCollider>();
        if(mesh_collider)
            mesh_collider.enabled = false;


        var animatorParent = GetComponentInParent<Animator>();
        //animatorParent.SetBool("IsDead");
        if(animatorParent)
            animatorParent.enabled = false;

        // Disable Nav
        agent.enabled = false;

        // Ragdoll script
        var ragdollScript = GetComponent<ragdoll>();
        if(ragdollScript)
            ragdollScript.EnableRagdoll();

        Invoke(nameof(DestroyEntity), 10f);
    }

    public void OnHurt()
    {
        if(!soundController.source_once.isPlaying)
            soundController.PlaySoundList(npc_hurt_clips);
    }

    private void Audio_Idle()
    {
        soundController.PlaySoundList(npc_idle_clips);
        idle_clip_played = false;
    }

    public void DestroyEntity()
    {
        Destroy(gameObject);
    }
}
