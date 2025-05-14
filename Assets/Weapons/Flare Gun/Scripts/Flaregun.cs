using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController;
using UnityEngine.Events;

public class Flaregun : MonoBehaviour
{
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform barrelEnd;
    [SerializeField] private Transform camera;
    [SerializeField] private PlayerInput playerInput;
    private PlayerCharacter playerCharacter;
    public int maxSpareRounds = 100;
    public int spareRounds = 100;
    public int currentRound = 1;
    public Rigidbody flareBullet;
    public float Damage = 30f;
    public float BulletRange = 100f;
    public int bulletSpeed = 2000;
    public GameObject muzzleParticles;
    public AudioClip flareShotSound;
    public AudioClip noAmmoSound;    
    public AudioClip reloadSound;
    //public UnityEvent OnFlareFired;
    public float flareJumpForce = 30f;
    public float minJumpAngle = 25f; 
    public float maxJumpDistance = 2f;
    public LayerMask groundLayer;

    private void Awake()
    {
        if (motor == null)
            motor = GetComponentInParent<KinematicCharacterMotor>();
        playerCharacter = motor.GetComponent<PlayerCharacter>();
    }

    private void Update()
    {
        if (playerInput.actions["Fire"].triggered && !GetComponent<Animation>().isPlaying)
        {
            if (currentRound > 0)
            {
                Shoot();
            }
            else
            {
                GetComponent<Animation>().Play("noAmmo");
                GetComponent<AudioSource>().PlayOneShot(noAmmoSound);
            }
        }

        if (playerInput.actions["Reload"].triggered && !GetComponent<Animation>().isPlaying)
        {
            Reload();
        }
    }

    void Shoot()
    {
        currentRound = Mathf.Max(0, --currentRound);

        GetComponent<Animation>().CrossFade("Shoot");
        GetComponent<AudioSource>().PlayOneShot(flareShotSound);
        Instantiate(muzzleParticles, barrelEnd.position, barrelEnd.rotation);
        Rigidbody bulletInstance = Instantiate(flareBullet, barrelEnd.position, barrelEnd.rotation);
        bulletInstance.AddForce(barrelEnd.forward * bulletSpeed * 10);
        TryFlareJump();

        if (Physics.Raycast(barrelEnd.position, barrelEnd.forward, out RaycastHit hitInfo, BulletRange))
        {
            if (hitInfo.collider.TryGetComponent(out Entity enemy))
            {
                enemy.Health -= Damage;
            }
        }
        //OnFlareFired?.Invoke();
    }

    void TryFlareJump()
    {
        if (playerCharacter == null) return;

        float angleFromUp = Vector3.Angle(Vector3.up, barrelEnd.forward);
        if (angleFromUp < minJumpAngle)
            return;

        if (Physics.Raycast(barrelEnd.position, barrelEnd.forward, out RaycastHit hit, maxJumpDistance, groundLayer))
        {
            Vector3 flareDirection = -barrelEnd.forward; 
            playerCharacter.StartFlareJump(flareDirection, flareJumpForce);
        }
    }

    void Reload()
    {
        if (spareRounds > 0 && currentRound == 0)
        {
            GetComponent<AudioSource>().PlayOneShot(reloadSound);            
            spareRounds--;
            currentRound++;
            GetComponent<Animation>().CrossFade("Reload");
        }
    }
}
