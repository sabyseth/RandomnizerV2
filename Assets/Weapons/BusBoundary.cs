using UnityEngine;

public class BusBoundary : MonoBehaviour
{
    public Vector3 boundarySize = new Vector3(100f, 0f, 100f); // Set your map size
    
    void LateUpdate()
    {
        // Get current position
        Vector3 pos = transform.position;
        
        // Clamp X position
        pos.x = Mathf.Clamp(pos.x, -boundarySize.x/2, boundarySize.x/2);
        
        // Clamp Z position
        pos.z = Mathf.Clamp(pos.z, -boundarySize.z/2, boundarySize.z/2);
        
        // Apply clamped position
        transform.position = pos;
    }

    // Visualize boundary in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, boundarySize);
    }
}