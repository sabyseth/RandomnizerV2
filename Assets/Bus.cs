using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController;
using UnityEngine.Events;

using UnityEngine;

public class Bus : MonoBehaviour
{
    public float speed = 10;
    public Vector3 moveDirection = Vector3.left;
    
    private void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }
}