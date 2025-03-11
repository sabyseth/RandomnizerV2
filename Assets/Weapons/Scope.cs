using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Scope : MonoBehaviour
{
    public Animator animator;
    public PlayerInput playerInput;
    private bool IsScoped = false;
    private InputAction fireAction;
    void Awake()
    {
        fireAction = playerInput.actions["Scope"];
    }
    void Update(){
         if (fireAction.WasPressedThisFrame()) 
        {
            IsScoped = !IsScoped;
            animator.SetBool("Scoped", IsScoped);
    }
}
}