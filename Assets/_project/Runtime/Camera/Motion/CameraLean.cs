using UnityEngine;

public class CameraLean : MonoBehaviour
{
    [SerializeField] private float attackDamping = 0.5f;
    [SerializeField] private float decayDamping = 0.3f;
    [SerializeField] private float walkStrength = 0.075f;
    [SerializeField] private float slideStrength = 0.2f;
    [SerializeField] private float strengthResponse = 5f;

    private Vector3 _dampedAcceleration;
    private Vector3 _dampedAccelerationVel;
    private float _smoothStrength;

    public void Initialize()
    {
        _smoothStrength = walkStrength;
    }

    public void UpdateLean(float deltaTime, bool sliding, Vector3 acceleration, Vector3 up)
    {
        
        Debug.DrawRay(transform.position, acceleration, Color.red);
        Debug.DrawRay(transform.position, _dampedAcceleration, Color.blue);


        var planarAcceleration = Vector3.ProjectOnPlane(acceleration, up);
        var damping = planarAcceleration.magnitude > _dampedAcceleration.magnitude
            ? attackDamping
            : decayDamping;

        _dampedAcceleration = Vector3.SmoothDamp
        (
            current: _dampedAcceleration,
            target: planarAcceleration,
            currentVelocity: ref _dampedAccelerationVel,
            smoothTime: damping,
            maxSpeed: float.PositiveInfinity,
            deltaTime: deltaTime
        );


        // Get the rotation axis based on the accelration vector.
        var leanAxis = Vector3.Cross(_dampedAcceleration.normalized, up).normalized;

        // Reset the rotation to that of its parent.
        transform.localRotation = Quaternion.identity;

        // Rotate around the lean axis.
        var targetStrength = sliding
            ? slideStrength
            : walkStrength;

        _smoothStrength = Mathf.Lerp(_smoothStrength, targetStrength, 1f - Mathf.Exp(-strengthResponse * deltaTime));

        transform.rotation = Quaternion.AngleAxis(-_dampedAcceleration.magnitude * _smoothStrength, leanAxis) * transform.rotation;
    }
}
