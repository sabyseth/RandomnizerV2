using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GunDamage : MonoBehaviour
{
    public PlayerInput playerInput;
    //[SerializeField] private Animator animator;
    private Animation anim;
    private InputAction fireAction;
    public float Damage;
    public Recoil RecoilObject;
    public float BulletRange;
    public ParticleSystem MuzzleFlash;
    private Transform PlayerCamera;
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;
    public TrailRenderer BulletTrail;
    public float recoilamount = 0.05f;
    public UnityEvent OnGunShoot;
    public float FireCoolDown;
    public bool Automatic;
    public float CurrentCoolDown;
    public bool burst;
    public bool revolver;
    public int BurstCount = 3; 
    public float BurstInterval = 0.1f; 
    private bool isBursting = false;
    public bool randum;

    private void Awake()
    {
        
        fireAction = playerInput.actions["Fire"];
        PlayerCamera = Camera.main.transform;
       // if (animator == null) animator = GetComponent<Animator>();
   // animator.SetBool("cocked", false);
        anim = GetComponent<Animation>();
        if (randum){
            FireCoolDown = Random.Range(0, 1); 
        }
    }

    private void Update()
    {
        if (fireAction.IsPressed()) 
        {
            if (CurrentCoolDown <= 0f)
            {
                if (burst == true){
                if (!isBursting) 
                {
                    StartCoroutine(BurstFire());
                }
                }
                if (revolver == true){
                    Debug.Log("revo");
                    anim.Play("Hammer");
                   // new WaitForSeconds(0.5f);
                Invoke("Shoot", .9f);
                CurrentCoolDown = FireCoolDown;
                }
                else {
                    Shoot();
               CurrentCoolDown = FireCoolDown;

                }
            }
        }

        
        CurrentCoolDown -= Time.deltaTime;
    }

    private IEnumerator BurstFire()
    {
        isBursting = true;

        for (int i = 0; i < BurstCount; i++)
        {
            Shoot();  
            yield return new WaitForSeconds(BurstInterval); 
        }

       
        CurrentCoolDown = FireCoolDown;
        isBursting = false;
    }

    public void Shoot()
    {
       // if (revolver == true){
        //     new WaitForSeconds (3.0f);
       // }
        MuzzleFlash.Play();
        RecoilObject.recoil += recoilamount;
        Ray gunRay = new Ray(bulletSpawnPoint.position, bulletSpawnPoint.forward);
        Debug.Log("Shot");
        //animator.SetBool("cocked", false);
        if (Physics.Raycast(gunRay, out RaycastHit hitInfo, BulletRange))
        {
            
            TrailRenderer trail = Instantiate(BulletTrail, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            StartCoroutine(SpawnTrail(trail, hitInfo.point)); 
            if (hitInfo.collider.gameObject.TryGetComponent(out Entity enemy))
            {
                enemy.Health -= Damage;
            }
        }
        else
        {
            
            Vector3 missPoint = gunRay.origin + gunRay.direction * BulletRange;
            TrailRenderer trail = Instantiate(BulletTrail, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            StartCoroutine(SpawnTrail(trail, missPoint)); 
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 targetPoint)
    {
        float distance = Vector3.Distance(Trail.transform.position, targetPoint);
        float startingDistance = distance;
        Vector3 startPosition = Trail.transform.position;

        while (distance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, targetPoint, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * bulletSpeed;

            yield return null;
        }

        Trail.transform.position = targetPoint;
    }
}
