using System.Collections;
using System.Collections.Generic;
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
        scopeOverlay.SetActive(false);
        shootAction = playerInput.actions["Fire"];
    }
    void Update(){
         if (fireAction.WasPressedThisFrame()) 
        {
            IsScoped = !IsScoped;
            animator.SetBool("Scoped", IsScoped);
            scopeOverlay.SetActive(IsScoped);
            if (IsScoped){
            StartCoroutine(OnScoped());
            }
            else{
            Unscoped();
            }
    }
          if (shootAction.WasPressedThisFrame() && IsScoped)
        {
            Unscoped();
            animator.SetBool("Scoped", false);
        }
}
void Unscoped(){
scopeOverlay.SetActive(false);
weaponCamera.SetActive(true);
mainCamera.fieldOfView = normalFOV;
}
IEnumerator OnScoped(){
        normalFOV = mainCamera.fieldOfView; 
        mainCamera.fieldOfView = scopedFOV; 
        scopeOverlay.SetActive(true);
        yield return new WaitForSeconds(0.075f); 
        weaponCamera.SetActive(false);
}
}