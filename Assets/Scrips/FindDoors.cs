using UnityEngine;
using System.Collections;

public class FindDoors : MonoBehaviour
{
    public Transform startPoint; // Assign this in the Inspector (e.g., Player)
    public Material lineMaterial;

    void Start()
    {
        StartCoroutine(FindDoorsWithDelay());
    }

    IEnumerator FindDoorsWithDelay()
    {
        // Wait for a few seconds to allow all objects to spawn
        yield return new WaitForSeconds(2f);

        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            // Check if the object's name is "door"
            if (obj.name.ToLower() == "door")
            {
                Debug.Log("Door found at position: " + obj.transform.position + ", Path: " + GetHierarchyPath(obj.transform));
                DrawPath(obj.transform.position);
            }
        }
    }

    void DrawPath(Vector3 doorPosition)
    {
        GameObject lineObj = new GameObject("PathToDoor");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, doorPosition);
    }

    string GetHierarchyPath(Transform obj)
    {
        string path = obj.name;
        while (obj.parent != null)
        {
            obj = obj.parent;
            path = obj.name + "/" + path;
        }
        return path;
    }
}
