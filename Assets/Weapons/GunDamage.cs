using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GunDamage : MonoBehaviour
{
    public PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction Reload;
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
    public int ammo; 
    public Text ammoDisplay;
    private int clip;
    
    public int BurstCount = 3; 
    public float BurstInterval = 0.1f; 
    private bool isBursting = false;

    private void Awake()
    {
   
        fireAction = playerInput.actions["Fire"];
        Reload = playerInput.actions["Reload"];
        PlayerCamera = Camera.main.transform;
        clip = ammo;  
    }

    private void Update()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.text = ammo.ToString();
        }
        if (fireAction.IsPressed()) 
        {
            if (CurrentCoolDown <= 0f && ammo > 0) 
            {
                if (burst == true){
                if (!isBursting) 
                {
                    StartCoroutine(BurstFire()); 
                }
                }
                else {

                    ammo--;
                    Shoot();
               CurrentCoolDown = FireCoolDown;

                }
            }
        }
        if (Reload.IsInProgress())
        {
            ammo =+ clip; 
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
        MuzzleFlash.Play();
        RecoilObject.recoil += recoilamount;
        Ray gunRay = new Ray(bulletSpawnPoint.position, bulletSpawnPoint.forward);
        Debug.Log("Shot");

        
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
