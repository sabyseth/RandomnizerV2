using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController;
using UnityEngine.Events;

public class GrapplingGun : MonoBehaviour
{
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private Transform gunTip;
    [SerializeField] private Transform camera;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGrappleable;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float baseGrappleSpeed = 25f; 
    [SerializeField] private float maxGrappleSpeed = 50f;
    [SerializeField] private float grappleAcceleration = 15f;
    public UnityEvent OnGrappleStopped;
    private Vector3 grapplePoint;
    private Vector3 currentGrapplePosition;
    private PlayerInput playerInput;
    private bool wasGrapplePressed = false;
    private float distance;

    private void Awake() 
    {
        if (lr == null) lr = GetComponent<LineRenderer>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        bool isGrapplePressed = playerInput.actions["Grapple"].IsPressed();
        if (isGrapplePressed && !wasGrapplePressed)
        {
            StartGrapple();
        }
        else if (!isGrapplePressed && wasGrapplePressed)
        {
            StopGrapple();
        }
        wasGrapplePressed = isGrapplePressed;
        if (lr.positionCount > 0 && Vector3.Distance(motor.TransientPosition, grapplePoint) < 3f)
        {
            StopGrapple();
        }
    }

    private void LateUpdate() 
    {
        DrawRope();
    }

    private void StartGrapple()
    {
        if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            currentGrapplePosition = gunTip.position;
            PlayerCharacter pc = player.GetComponent<PlayerCharacter>();
            if (pc != null)
            {
                pc.grappleSpeed = baseGrappleSpeed;
                pc.maxGrappleSpeed = maxGrappleSpeed; 
                pc.grappleAcceleration = grappleAcceleration;
                pc.StartGrapple(hit.point);
            }
            lr.positionCount = 2;
        }
    }

    public void StopGrapple()
    {
        PlayerCharacter pc = player.GetComponent<PlayerCharacter>();
        if (pc != null)
        {
            pc.StopGrapple();
        }
        lr.positionCount = 0;
        OnGrappleStopped?.Invoke();
    }

    private void DrawRope() 
    {
        if (lr == null || gunTip == null || lr.positionCount != 2) return;
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }
    public Vector3 GetGrapplePoint() => grapplePoint;
}