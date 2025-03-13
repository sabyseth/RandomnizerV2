using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Scope : MonoBehaviour
{
    public Animator animator;
    public PlayerInput playerInput;

    public GameObject scopeOverlay;
    private bool IsScoped = false;
    private InputAction fireAction;
    void Awake()
    {
        fireAction = playerInput.actions["Scope"];
        scopeOverlay.SetActive(false);
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
}
void Unscoped(){
scopeOverlay.SetActive(false);
}
IEnumerator OnScoped(){
    yield return new WaitForSeconds(.15f);
    scopeOverlay.SetActive(true);
}
}