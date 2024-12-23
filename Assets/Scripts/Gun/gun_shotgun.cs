using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

using Random = UnityEngine.Random;

public class gun_shoot_shotgun : MonoBehaviour
{
    // Weapon specific settings
    public float damage = 3;
    public float range = 320;
    public float ammo = 64;
    public float ammo_max = 999;
    public int bullets_per_shot = 8;
    public float bullet_speed = 100f;

    // Bullet effects
    public Transform muzzle;
    public Vector3 bullet_spread = new Vector3(0.02f, 0.02f, 0.02f);
    public VisualEffect particle_bullet_impact;
    public VisualEffect particle_shoot_system;
    public TrailRenderer particle_bullet_trail;
    public LayerMask Mask;

    private Animator animator;


    public Transform PlayerPrediction;
    public sound_controller_generic soundController;

    private CharacterController controllerPrediction;

    // UI
    public TMP_Text mainUI_Ammo;

    // Start is called before the first frame update
    void Start()
    {
        controllerPrediction = PlayerPrediction.GetComponent<CharacterController>();;
        soundController = GetComponent<sound_controller_generic>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        mainUI_Ammo.text = ammo.ToString();
    }

    // The main shoot function
    public void Shoot()
    {
        //Debug.Log("shot shotgun!");
        if(ammo <= 0)
            return;

        animator.SetTrigger("IsShooting");
        particle_shoot_system.Play();

        Shoot_Audio();         
       
        for(int i = 0; i < bullets_per_shot; i++)
        {
            var muzzle_predicted = muzzle.position + 5*controllerPrediction.velocity*Time.deltaTime;
            var bullet_dir = GetDirection();
            Ray gunray = new Ray(muzzle_predicted, bullet_dir);

            if(Physics.Raycast(gunray, out RaycastHit hit, range, Mask)){
                TrailRenderer trail = Instantiate(particle_bullet_trail, muzzle_predicted, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));

                if(hit.collider != null){
                    var hitRigid = hit.collider.gameObject.GetComponent<Rigidbody>();
                    if(hitRigid)
                        hitRigid.AddForce(bullet_dir.normalized*1000f);

                    entity_generic enemy = hit.collider.GetComponent<entity_generic>();
                    if(!enemy){
                        enemy = hit.collider.GetComponentInParent<entity_generic>();
                    }

                    if(enemy){
                        enemy.Health -= damage;
                    }
                }
            }
            else
            {
                TrailRenderer trail = Instantiate(particle_bullet_trail, muzzle_predicted, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, muzzle_predicted + bullet_dir * 100, Vector3.zero, false));

            }
        }


        // Remove ammo
        ammo -= 1;
    }


    // Weapon audio section
    public List<AudioClip> clip_shoot;
    public List<AudioClip> clip_reload;

    private void Shoot_Audio()
    {
        soundController.PlaySoundList(clip_shoot);
    }

    public void Reload(int amount)
    {
        ammo += Math.Clamp(amount, 0, ammo_max);   
        soundController.PlaySoundList(clip_reload);
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = muzzle.forward;
        
        direction += new Vector3(
            Random.Range(-bullet_spread.x, bullet_spread.x),
            Random.Range(-bullet_spread.y, bullet_spread.y),
            Random.Range(-bullet_spread.z, bullet_spread.z)
        );
        direction.Normalize();
        return direction;
    } 

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while(remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1f - (remainingDistance / distance));
            remainingDistance -= bullet_speed * Time.deltaTime;

            yield return null;
        }

        Trail.transform.position = HitPoint;
        if(MadeImpact){
            var impact = Instantiate(particle_bullet_impact, HitPoint+HitNormal*0.01f, Quaternion.LookRotation(HitNormal));
            Destroy(impact.gameObject, 1.5f); // Fixed at 1.5 second
        }

        Destroy(Trail.gameObject, Trail.time);
    }

}
