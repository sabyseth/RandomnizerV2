using UnityEngine;

public class Recoil : MonoBehaviour {

    public float maxRecoil_x = -20.0f;
    public float maxRecoil_y = -10.0f;

    public float maxTrans_x = 1.0f;
    public float maxTrans_z = -1.0f;

    public float recoilSpeed = 10.0f;
    public float recoil = 0.0f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        // Store the original position and rotation at the start
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        if (recoil > 0)
        {
            // Apply the recoil effect (rotation and translation)
            var maxRecoil = Quaternion.Euler(
                Random.Range(transform.localRotation.x, maxRecoil_x),
                Random.Range(transform.localRotation.y, maxRecoil_y),
                transform.localRotation.z);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);

            var maxTranslation = new Vector3(
                Random.Range(transform.localPosition.x, maxTrans_x),
                transform.localPosition.y,
                Random.Range(transform.localPosition.z, maxTrans_z));

            transform.localPosition = Vector3.Slerp(transform.localPosition, maxTranslation, Time.deltaTime * recoilSpeed);

            recoil -= Time.deltaTime;
        }
        else
        {
            // Reset recoil to 0 and return the gun to the original position and rotation smoothly
            recoil = 0;

            // Smoothly return to original rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, Time.deltaTime * recoilSpeed / 2);

            // Smoothly return to original position
            transform.localPosition = Vector3.Slerp(transform.localPosition, originalPosition, Time.deltaTime * recoilSpeed / 2);
        }
    }
}
