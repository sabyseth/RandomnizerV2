using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;
    private Vector3 _eulerAngles;
    public float maxPitch = 90f;

    public void Initialize(Transform target)
    {
        Debug.Log("Intitializig Camera");
        if (!IsOwner) { Debug.Log("Camera Initiliazation Stopped (Not the owner)"); return; }
        if (!IsLocalPlayer) { Debug.Log("Camera Initiliazation Stopped (Not the local player)"); return; }
        // if(!IsHost) return;
        // if(!IsLocalPlayer) return; 
        transform.position = target.position;
        transform.eulerAngles = _eulerAngles = target.eulerAngles;

    }

    public void UpdateRotation(CameraInput input)
    {
        // Update the pitch (X-axis) and yaw (Y-axis)
        _eulerAngles.x += -input.Look.y * sensitivity;
        _eulerAngles.y += input.Look.x * sensitivity;

        // Clamp the pitch to stay within the allowed range
        _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, -maxPitch, maxPitch);

        // Apply the clamped rotation
        transform.eulerAngles = _eulerAngles;
    }


    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.up, .5f);
        //Gizmos.DrawSphere(, 0.4f);
    }
}
