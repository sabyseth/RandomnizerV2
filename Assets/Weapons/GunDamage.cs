using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDamage : MonoBehaviour
{
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

    private void Start()
    {
        PlayerCamera = Camera.main.transform;
    

   }

   public void Shoot()
{
    MuzzleFlash.Play();
    RecoilObject.recoil += recoilamount;
    Debug.Log("Shot");

    // Create the ray using the camera's forward direction
    Ray gunRay = new Ray(bulletSpawnPoint.position, bulletSpawnPoint.forward);
    Debug.DrawRay(PlayerCamera.position, PlayerCamera.forward, Color.green);

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
