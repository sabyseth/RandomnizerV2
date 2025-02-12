using UnityEngine;
using KinematicCharacterController;
using UnityEngine.Events;

public enum CrouchInput
{
    None, Toggle
}

public enum SprintInput
{
    None, Toggle
}

public enum Stance
{
    Stand, Crouch, Slide, Sprint
}

public struct CharacterState
{
    public bool Grounded;
    public Stance Stance;
    public Vector3 Velocity;
    public Vector3 Acceleration;
}

public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector2 Move;
    public bool Jump;
    public bool JumpSustain;
    public CrouchInput Crouch;
    public SprintInput Sprint;
    public bool Fire;
}
public class PlayerCharacter : MonoBehaviour, ICharacterController
{

    public UnityEvent OnGunShoot;
    public float FireCoolDown;
    public bool Automatic;
    public float CurrentCoolDown;
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [SerializeField] private Transform cameraTarget;
    [Space]
    [SerializeField] private CharacterInfo characterInfo;
    [Space]
    [SerializeField] private float sprintSpeed = 35f;
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private float crouchSpeed = 7f;
    [SerializeField] private float sprintResponse = 30f;
    [SerializeField] private float walkResponse = 25f;
    [SerializeField] private float crouchResponse = 20f;
    [Space]

    [SerializeField] private float airSpeed = 15f;
    [SerializeField] private float airAcceleration = 70f;
    [Space]
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float coyoteTime = 0.2f;
    [Range(0f, 1f)]
    [SerializeField] private float jumpSustainGravity = 0.4f;
    [SerializeField] private float gravity = -90f;
    [Space]
    [SerializeField] private float slideStartSpeed = 25f;
    [SerializeField] private float slideEndSpeed = 15f;
    [SerializeField] private float slideFriction = 0.8f;
    [SerializeField] private float slideSteerAcceleration = 5f;
    [SerializeField] private float slideGravity = -90f;
    [Space]
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchHeightResponse = 15f;
    [Range(0f, 1f)]

    [SerializeField] private float standCameraTargetHeight = 0.9f;
    [Range(0f, 1f)]
    [SerializeField] private float crouchCameraTargetHeight = 0.7f;


    private CharacterState _state;
    private CharacterState _lastState;
    private CharacterState _tempState;
    
    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;
    private bool _requestedStustainedJump;

    private bool _requestedCrouch;
    private bool _requestedCrouchInAir;

    private float _timeSinceUngrounded;
    private float _timeSinceJumpRequest;
    private bool _ungroundedDueToJump;

    private Collider[] _uncrouchOverlapResults; 
    public void Initialize()
    {
        _state.Stance = Stance.Stand;
        _lastState = _state;

        _uncrouchOverlapResults = new Collider[8];

        motor.CharacterController = this;

        CurrentCoolDown = FireCoolDown;
    }

    public void UpdateInput(CharacterInput input)
    {
        if (input.Sprint == SprintInput.Toggle)
        {
            _state.Stance = Stance.Sprint;
        }
        else if (_state.Stance == Stance.Sprint)
        {
            _state.Stance = Stance.Stand; // Fallback if Sprint is released
        }
         if (input.Fire)
             {
                 if (CurrentCoolDown <= 0f)
                 {
                     OnGunShoot?.Invoke();
    
                     CurrentCoolDown = FireCoolDown;
                 }
             }
             CurrentCoolDown -= Time.deltaTime;
        _requestedRotation = input.Rotation;
        // Take the 2d input veector and create a 3d movement vector on the XZ plane.
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        // clamp the lenghth to 1 to prevent moving faster diagonally was WASD inoput.
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);
        // Orient the input so it's relative to the direction te player is facing.
        _requestedMovement = input.Rotation * _requestedMovement;

        var wasRequestingJump = _requestedJump;
        _requestedJump = _requestedJump || input.Jump;
        if (_requestedJump && !wasRequestingJump)
            _timeSinceJumpRequest = 0f;

        _requestedStustainedJump = input.JumpSustain;

        var wasRequestingCrouch = _requestedCrouch;
        //_requestedCrouch = input.Crouch;
        _requestedCrouch = input.Crouch switch
        {
            CrouchInput.Toggle => true,
            CrouchInput.None => false,
            _ => _requestedCrouch
        };
        // _requestedCrouch = input.Crouch switch
        // {
        //     CrouchInput.Toggle => !_requestedCrouch,
        //     CrouchInput.None => _requestedCrouch,      TOGGLE CROUCH
        //     _ => _requestedCrouch
        // };
        if (_requestedCrouch && !wasRequestingCrouch)
            _requestedCrouchInAir = !_state.Grounded;
        else if (!_requestedCrouch && wasRequestingCrouch)
            _requestedCrouchInAir = false;
    }

    public void UpdateBody(float deltaTime)
    {
        var currentHeight = motor.Capsule.height;
        var normalizedHeight = currentHeight / standHeight;

        var cameraTargetHeight = currentHeight *
        (
            _state.Stance is Stance.Stand
                ? standCameraTargetHeight
                : crouchCameraTargetHeight
        );
        var rootTargetScale = new Vector3(1f, normalizedHeight, 1f);

        cameraTarget.localPosition = Vector3.Lerp
        (
            a: cameraTarget.localPosition,
            b: new Vector3(0f, cameraTargetHeight, 0f),
            t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
        );
        root.localScale = Vector3.Lerp
        (
            a: root.localScale,
            b: rootTargetScale,
            t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
        );

    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        characterInfo.SetStateText(_state.Stance.ToString());   
        characterInfo.SetFloatValue(currentVelocity.magnitude);
        _state.Acceleration = Vector3.zero;

        // If on the ground...
        if (motor.GroundingStatus.IsStableOnGround)
        {
            _timeSinceUngrounded = 0f;
            _ungroundedDueToJump = false;

            // Snap the requested movement direction to the angle of the surface
            // the character is curerntly walking on.
            var groundedMovement = motor.GetDirectionTangentToSurface
            (
                direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;
            //Start sliding.
            {
                var moving = groundedMovement.sqrMagnitude > 0f;
                var crouching = _state.Stance is Stance.Crouch;
                var wasStanding = _lastState.Stance is Stance.Stand; // potential cause for phantom slide boost if it happens
                var wasInAir = !_lastState.Grounded;
                if(moving && crouching && (wasStanding || wasInAir))
                {
                    _state.Stance = Stance.Slide;

                    // When landing on stable ground the character motor projects the velocity onto a flat ground plane.
                    // See: KinematicCharacterMotor.HandleVelocityProjection()
                    // This is normally good, because under normal circumstatnes the player shouldn't slide when landing on the ground.
                    // In this case, we *want* the player to slide.
                    // Reproject the last frames (falling) veloctiy onto the ground normal to slide.
                    if (wasInAir)
                    {
                        currentVelocity = Vector3.ProjectOnPlane
                        (
                            vector: _lastState.Velocity,
                            planeNormal: motor.GroundingStatus.GroundNormal
                        );
                    }

                    var effectiveSlideStartSpeed = slideStartSpeed;
                    if (!_lastState.Grounded && !_requestedCrouchInAir)
                    {
                        effectiveSlideStartSpeed = 0f;
                        _requestedCrouchInAir = false;
                    }

                    var slideSpeed = Mathf.Max(effectiveSlideStartSpeed, currentVelocity.magnitude);
                    currentVelocity = motor.GetDirectionTangentToSurface
                    (
                        direction: currentVelocity,
                        surfaceNormal: motor.GroundingStatus.GroundNormal
                    ) * slideSpeed;

                    Debug.DrawRay(transform.position, currentVelocity, Color.green, 5f);

                }
            }
            // Move.


            if (_state.Stance is Stance.Stand or Stance.Crouch or Stance.Sprint)
            {       
                // Calculate the speed and responsiveness of movement based
                // on the character's stance.
                var speed = _state.Stance switch
                {
                    Stance.Stand => walkSpeed,
                    Stance.Crouch => crouchSpeed,
                    Stance.Sprint => sprintSpeed,
                    _ => walkSpeed
                };

                var response = _state.Stance switch
                {
                    Stance.Stand => walkResponse,
                    Stance.Crouch => crouchResponse,
                    Stance.Sprint => sprintResponse,
                    _ => walkResponse
                };
                // And smoothly move along the ground in that direction.
                var targetVelocity = groundedMovement * speed;
                var moveVelocity = Vector3.Lerp
                (
                    a: currentVelocity,
                    b: targetVelocity,
                    t: 1f - Mathf.Exp(-response * deltaTime)
                );
                
                _state.Acceleration = (moveVelocity - currentVelocity) / deltaTime;
                
                currentVelocity = moveVelocity;
            } 
            // Continue Sliding.
            else
            {
                _timeSinceUngrounded += deltaTime;
                // Friction
                currentVelocity -= currentVelocity * (slideFriction * deltaTime);

                // Slope.
                {
                    var force = Vector3.ProjectOnPlane
                    (
                        vector: -motor.CharacterUp,
                        planeNormal: motor.GroundingStatus.GroundNormal
                    ) * slideGravity;

                    currentVelocity -= force * deltaTime;
                }

                // Steer.
                {
                    // Target velocity is the player's movement direction, at the current speed.
                    var currentSpeed = currentVelocity.magnitude;
                    var targetVelocity = groundedMovement * currentSpeed;
                    var steerVelocity = currentVelocity;
                    var steerForce = (targetVelocity - steerVelocity) * slideSteerAcceleration * deltaTime;
                    // Add steer force, but clamp velocity so the slide sped doesn't increase due to direct movement input.
                    // to direct movement input.
                    steerVelocity += steerForce;
                    steerVelocity = Vector3.ClampMagnitude(steerVelocity, currentSpeed);

                    _state.Acceleration = (steerVelocity - currentVelocity) / deltaTime;

                    currentVelocity = steerVelocity;
                }

                // Stop.
                if (currentVelocity.magnitude < slideEndSpeed)
                    _state.Stance = Stance.Crouch;
            }
        }
        // else, in the air...
        else
        {
            // Move.
            if (_requestedMovement.sqrMagnitude > 0f)
            {
                // requested movement projected onto movement plane. (magnitude preserved)
                var planarMovement = Vector3.ProjectOnPlane
                (
                    vector: _requestedMovement,
                    planeNormal: motor.CharacterUp
                ) * _requestedMovement.magnitude;

                // Current Velocity on movement plane.
                var currentPlanarVelocity = Vector3.ProjectOnPlane
                (
                    vector: currentVelocity,
                    planeNormal: motor.CharacterUp
                ) * _requestedMovement.magnitude;

                // Calculate movement force.
                // Will be changed depending on current velocity.
                var movementForce = planarMovement * airAcceleration * deltaTime;

                // If moving slower than the max air speed, treat movementForce as a simple steering force.
                if (currentPlanarVelocity.magnitude < airSpeed)
                {
                    // Add it to the current planar velcotiry for a target velocitty
                    var targetPlanarVelocity = currentPlanarVelocity + movementForce;
                    // Limit target velocity to air speed.
                    targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);

                    // Steer towards target velocity.
                    movementForce = targetPlanarVelocity - currentPlanarVelocity;
                }

                // Otherwise, nerf the movment orce when it is in the direction of the current planar velocity
                // to prevent acceleration further beyond max air speed.
                else if (Vector3.Dot(currentPlanarVelocity, movementForce) > 0f)
                {
                    // project movement force onto the plane whose normal is the current planar velocity.
                    var constrainedMovementForce = Vector3.ProjectOnPlane
                    (
                        vector: movementForce,
                        planeNormal: currentPlanarVelocity.normalized
                    );

                    movementForce = constrainedMovementForce;
                }

                // Prevent air-climbing steep slopes.
                if (motor.GroundingStatus.FoundAnyGround)
                {
                    // if moving in the same direction as the resutlant velocity...
                    if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f)
                    {
                        // Calculate obstruction normal.
                        var obstructionNormal = Vector3.Cross
                        (
                            motor.CharacterUp,
                            Vector3.Cross
                            (
                                motor.CharacterUp,
                                motor.GroundingStatus.GroundNormal
                            )
                        ).normalized;

                        // Project movement force onto obstruction plane.
                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);
                    }
                }

                currentVelocity += movementForce;
            }
            //Gravity
            var effectiveGravity = gravity;
            var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if (_requestedStustainedJump && verticalSpeed > 0f)
                effectiveGravity *= jumpSustainGravity;
            currentVelocity += motor.CharacterUp * effectiveGravity * deltaTime;
        }

        if (_requestedJump)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;
            var canCoyoteJump = _timeSinceUngrounded < coyoteTime && !_ungroundedDueToJump;

            if (grounded || canCoyoteJump)
            {
                _requestedJump = false; // Unset jump request.
                _requestedCrouch = false; // and request the character uncrouches. 
                _requestedCrouchInAir = false;
                // Unstick the palyer from the ground.
                motor.ForceUnground(time: 0.1f);
                _ungroundedDueToJump = true;

                // set minimum vertical speed to the jump speed.
                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
                // Add the difference in current and target vertical speed to the character's velocity.
                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            }
            else
            {
                _timeSinceJumpRequest += deltaTime;

                // Defer the jump request until coyote time has passed. 
                var canJumpLater = _timeSinceJumpRequest < coyoteTime;
                _requestedJump = canJumpLater;
            }
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        // Update the character's rotation to face in the same direction as the
        // requested rotation (camera rotation).

        // We don't want the character to pitch up and down, so the direction the character
        // looks should always be "flattened."

        // This is done by projecting a vector poingin in the same direction that
        // the player is looking onto a flay ground plane.

        var forward = Vector3.ProjectOnPlane
        (
            _requestedRotation * Vector3.forward,
            motor.CharacterUp
        );

        if (forward != Vector3.zero)
            currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        _tempState = _state;

        // Crouch.
        if (_requestedCrouch && _state.Stance is Stance.Stand)
        {
            _state.Stance = Stance.Crouch;
            motor.SetCapsuleDimensions
            (
                radius: motor.Capsule.radius,
                height: crouchHeight,
                yOffset: crouchHeight * 0.5f
            );
        }
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        if (!motor.GroundingStatus.IsStableOnGround && _state.Stance is Stance.Slide)
            _state.Stance = Stance.Crouch;
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        // Uncrouch.
        if (!_requestedCrouch && _state.Stance is not Stance.Stand)
        {
            // Tentatively "standup" the character capsule.
            motor.SetCapsuleDimensions
            (
                radius: motor.Capsule.radius,
                height: standHeight,
                yOffset: standHeight * 0.5f
            );

            // Then see if the capsule overlapse any colliders before actually
            // allowing the character to standup.
            var pos = motor.TransientPosition;
            var rot = motor.TransientRotation;
            var mask = motor.CollidableLayers;
            if (motor.CharacterOverlap(pos, rot , _uncrouchOverlapResults, mask, QueryTriggerInteraction.Ignore) > 0)
            {
                // Re-crouch.
                _requestedCrouch = true;
                motor.SetCapsuleDimensions
                (
                    radius: motor.Capsule.radius,
                    height: crouchHeight,
                    yOffset: crouchHeight * 0.5f
                );
            }
            else
            {
                _state.Stance = Stance.Stand;
            }
        }

        // Update state to reflect motor properties.
        _state.Grounded = motor.GroundingStatus.IsStableOnGround;
        _state.Velocity = motor.Velocity;
        // And update the _lastState to store the character state snapshot taken at
        // the beginning of this character update.
        _lastState = _tempState;
    }


    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}

    public bool IsColliderValidForCollisions (Collider coll) => true;

    public void OnDiscreteCollisionDetected (Collider hitCollider){}

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport){}

    public Transform GetCameraTarget() => cameraTarget;
    public CharacterState GetState() => _state;
    public CharacterState GetLastState() => _lastState;

    public void SetPosition(Vector3 position, bool killVelocity = true)
    {
        motor.SetPosition(position);
        if (killVelocity)
            motor.BaseVelocity = Vector3.zero;
    }

}
