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
    
      private void Awake()
    {
        // Get the Fire action from the input asset
        fireAction = playerInput.actions["Fire"];
        PlayerCamera = Camera.main.transform;

        
    }

   private void Update()
    {
        // Check if the fire action is triggered
        if (fireAction.IsPressed()) // Use IsPressed for continuous fire when holding
        {
            if (CurrentCoolDown <= 0f)
            {
                Shoot(); // Trigger the shooting event
                CurrentCoolDown = FireCoolDown; // Reset cooldown after shooting
            }
        }

        // Decrease cooldown each frame
        CurrentCoolDown -= Time.deltaTime;
    }
  public void Shoot(){
   CurrentCoolDown = FireCoolDown;
   MuzzleFlash.Play();
   RecoilObject.recoil += recoilamount;
   Ray gunRay = new Ray(bulletSpawnPoint.position, bulletSpawnPoint.forward);
   Debug.Log("Shot");
   //Debug.DrawRay(PlayerCamera.position, PlayerCamera.forward);
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
           //Debug.Log("Hit entity");
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
