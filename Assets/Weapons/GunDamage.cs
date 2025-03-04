using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GunDamage : MonoBehaviour
{
    public PlayerInput playerInput;
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

    // New variables for burst fire
    public int BurstCount = 3; // Number of rounds in the burst
    public float BurstInterval = 0.1f; // Time between each burst shot
    private bool isBursting = false;

    private void Awake()
    {
        // Get the Fire action from the input asset
        fireAction = playerInput.actions["Fire"];
        PlayerCamera = Camera.main.transform;
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
                    StartCoroutine(BurstFire()); // Start burst fire when not already bursting
                }
                }
                else {
                    Shoot();
               CurrentCoolDown = FireCoolDown;

                }
            }
        }

        // Decrease cooldown each frame
        CurrentCoolDown -= Time.deltaTime;
    }

    private IEnumerator BurstFire()
    {
        isBursting = true;

        for (int i = 0; i < BurstCount; i++)
        {
            Shoot();  // Shoot a single shot
            yield return new WaitForSeconds(BurstInterval);  // Wait before firing the next shot
        }

        // Reset cooldown after burst fire
        CurrentCoolDown = FireCoolDown;
        isBursting = false;
    }

    public void Shoot()
    {
        MuzzleFlash.Play();
        RecoilObject.recoil += recoilamount;
        Ray gunRay = new Ray(bulletSpawnPoint.position, bulletSpawnPoint.forward);
        Debug.Log("Shot");

        // Check if the ray hits anything
        if (Physics.Raycast(gunRay, out RaycastHit hitInfo, BulletRange))
        {
            // Spawn the bullet trail
            TrailRenderer trail = Instantiate(BulletTrail, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            StartCoroutine(SpawnTrail(trail, hitInfo.point)); // Use the hit point
            // Check if the hit object has an Entity component
            if (hitInfo.collider.gameObject.TryGetComponent(out Entity enemy))
            {
                enemy.Health -= Damage;
                // Debug.Log("Hit entity");
            }
        }
        else
        {
            // If no object is hit, calculate the endpoint based on the bullet range
            Vector3 missPoint = gunRay.origin + gunRay.direction * BulletRange;

            // Spawn the bullet trail
            TrailRenderer trail = Instantiate(BulletTrail, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            StartCoroutine(SpawnTrail(trail, missPoint)); // Use the calculated miss point
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
