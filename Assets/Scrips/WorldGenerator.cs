using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public static List<GameObject> GeneratedTiles = new List<GameObject>();

    [SerializeField] private GameObject tilePrefab;
    private int baseRadius = 128;
    private int adjustedRadius;
    private List<Vector3> doorPositions = new List<Vector3>();

    void Start()
    {
        StartCoroutine(AdjustRadiusAndGenerateWorld());
    }

    IEnumerator AdjustRadiusAndGenerateWorld()
    {
        yield return StartCoroutine(FindDoors());
        adjustedRadius = CalculateAdjustedRadius();
        GenerateWorld(adjustedRadius);
    }

    IEnumerator FindDoors()
    {
        yield return new WaitForSeconds(2f); // Wait for objects to spawn
        
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower() == "door")
            {
                doorPositions.Add(obj.transform.position);
            }
        }
    }

    int CalculateAdjustedRadius()
    {
        if (doorPositions.Count == 0)
            return baseRadius;

        float maxDistance = 0f;
        foreach (var door in doorPositions)
        {
            float distance = Vector3.Distance(Vector3.zero, door);
            if (distance > maxDistance)
            {
                maxDistance = distance;
            }
        }
        return Mathf.CeilToInt(maxDistance / 1.5f) + 10;
    }

    void GenerateWorld(int radius)
    {
        for (int x = 0; x < radius; x++)
        {
            for (int z = 0; z < radius; z++)
            {
                GameObject tile = Instantiate(tilePrefab,
                    new Vector3(x * 1.5f, 0, z * 1.5f),
                    Quaternion.identity);

                GeneratedTiles.Add(tile);
            }
        }

        // Create Path Object
        Path pathGenerator = new Path(radius, GeneratedTiles);
        pathGenerator.AssignDoorTiles(doorPositions);
        pathGenerator.GeneratePath();

        List<GameObject> pathTiles = pathGenerator.GetGeneratedPath;

        // Disable all tiles that are NOT in the path
        foreach (var tile in GeneratedTiles)
        {
            if (!pathTiles.Contains(tile))
            {
                tile.SetActive(false);
            }
        }
    }

}
