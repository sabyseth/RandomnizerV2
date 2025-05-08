using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

public class Scope : MonoBehaviour
{
    public Animator animator;
    public PlayerInput playerInput;
    public Camera mainCamera;
    public GameObject scopeOverlay;
    public GameObject weaponCamera;
    public float scopedFOV = 15f;
    private float normalFOV;
    private bool IsScoped = false;
    private InputAction fireAction;
    private InputAction shootAction;

    void Awake()
    {
        fireAction = playerInput.actions["Scope"];
        shootAction = playerInput.actions["Fire"];
        scopeOverlay.SetActive(false);
    }

    void Update()
    {
        if (fireAction.WasPressedThisFrame())
        {
            if (!IsScoped)
            {
                IsScoped = true;
                animator.SetBool("Scoped", IsScoped);
                scopeOverlay.SetActive(true); 
                StartCoroutine(OnScoped());
            }
            else
            {
                Unscoped();
            }
        }
        if (shootAction.WasPressedThisFrame() && IsScoped)
        {
            Unscoped();
        }
    }

    void Unscoped()
    {
        IsScoped = false;
        scopeOverlay.SetActive(false);  
        weaponCamera.SetActive(true);
        mainCamera.fieldOfView = normalFOV;
        animator.SetBool("Scoped", false);
    }

    IEnumerator OnScoped()
    {
        normalFOV = mainCamera.fieldOfView;
        mainCamera.fieldOfView = scopedFOV;
        weaponCamera.SetActive(false);
        yield return new WaitForSeconds(0.075f);
    }
}
