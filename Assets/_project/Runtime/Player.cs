using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [Space]
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;
    [Space]
//[SerializeField] private Volume volume;
    //[SerializeField] private StanceVignette stanceVignette;

    private PlayerInputActoions _inputActions;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _inputActions = new PlayerInputActoions();
        _inputActions.Enable();

        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());

        cameraSpring.Initialize();
        cameraLean.Initialize();

        //stanceVignette.Initialize(volume.profile);
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    void Update()
    {  
        var input = _inputActions.Gameplay;
        var deltaTime = Time.deltaTime;

        //Get camera input and update its rotation.
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

        // Get Character input and update it
        var characterInput = new CharacterInput
        {
            Rotation    = playerCamera.transform.rotation,
            Move        = input.Move.ReadValue<Vector2>(),
            Jump        = input.Jump.WasPressedThisFrame(),
            JumpSustain = input.Jump.IsPressed(),
            Crouch      = input.Crouch.IsPressed()
                ? CrouchInput.Toggle
                : CrouchInput.None,
            Sprint      = input.Sprint.IsPressed()
                ? SprintInput.Toggle
                : SprintInput.None,
            Fire        = input.Fire.WasPressedThisFrame()
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);

        #if UNITY_EDITOR
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if(Physics.Raycast(ray, out var hit))
            {
                Teleport(hit.point);
            } 
        }
        #endif
    }

    void LateUpdate()
    {
        var deltaTime = Time.deltaTime;
        var cameraTarget = playerCharacter.GetCameraTarget();
        var state = playerCharacter.GetState();

        playerCamera.UpdatePosition(cameraTarget);
        cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
        cameraLean.UpdateLean
        (
            deltaTime, 
            state.Stance is Stance.Slide, 
            state.Acceleration, 
            cameraTarget.up
        );

        //stanceVignette.UpdateVignette(deltaTime, state.Stance);
    }

    public void Teleport(Vector3 position)
    {
        playerCharacter.SetPosition(position);
        playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
    }
}
