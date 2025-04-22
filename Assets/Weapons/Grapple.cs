using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController;

public class GrapplingGun : MonoBehaviour {


[SerializeField] private KinematicCharacterMotor motor;   private LineRenderer lr;
   private Vector3 grapplePoint;
   public LayerMask whatIsGrappleable;
   public Transform gunTip, camera, player;
   private float maxDistance = 100f;
   private bool wasGrapplePressed = false;
   public float baseGrappleSpeed = 25f; 
   public float maxGrappleSpeed = 50f;
   public float grappleAcceleration = 15f;

   private PlayerInput playerInput;


   void Awake() {
       lr = GetComponent<LineRenderer>();
       playerInput = GetComponent<PlayerInput>();
   }


  void Update()
{
   bool isGrapplePressed = playerInput.actions["Grapple"].IsPressed();
  Debug.Log($"Grapple input: {playerInput.actions["Grapple"].ReadValue<float>()}");
   if (isGrapplePressed && !wasGrapplePressed)
   {
      
       StartGrapple();
   }
   else if (!isGrapplePressed && wasGrapplePressed)
   {
      
       StopGrapple();
   }
  
   wasGrapplePressed = isGrapplePressed;
}


   //Called after Update
   void LateUpdate() {
       DrawRope();
   }


void StartGrapple()
{
    if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, maxDistance, whatIsGrappleable))
    {
        grapplePoint = hit.point; // Store for rope drawing
        
        PlayerCharacter pc = player.GetComponent<PlayerCharacter>();
        if (pc != null)
        {
            pc.grappleSpeed = baseGrappleSpeed;
            pc.maxGrappleSpeed = maxGrappleSpeed; 
            pc.grappleAcceleration = grappleAcceleration;
            
            pc.StartGrapple(hit.point);
        }
        
        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;
        grapplePoint = hit.point; 
    }
}

void StopGrapple()
{
    PlayerCharacter pc = player.GetComponent<PlayerCharacter>();
    if (pc != null)
    {
        pc.StopGrapple();
    }
    lr.positionCount = 0;
}


   private Vector3 currentGrapplePosition;
  
   void DrawRope() {
       //If not grappling, don't draw rop
    if (lr == null || gunTip == null) return;
    if (lr.positionCount != 2) return;
    
  
       currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
      
       lr.SetPosition(0, gunTip.position);
       lr.SetPosition(1, currentGrapplePosition);
   }



   public Vector3 GetGrapplePoint() {
       return grapplePoint;
   }
}

