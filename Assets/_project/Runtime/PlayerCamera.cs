using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;
    private Vector3 _eulerAngles;
    public float maxPitch = 90f;

    public void Initialize(Transform target)
    {
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
}
