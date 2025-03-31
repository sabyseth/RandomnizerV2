using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public static List<GameObject> GeneratedTiles = new List<GameObject>();

    [SerializeField] private GameObject tilePrefab;
    private int fixedRadius = 160; // Fixed terrain size
    private List<Vector3> doorPositions = new List<Vector3>();
    private List<GameObject> pathTiles = new List<GameObject>(); // Store path tiles

    void Start()
    {
        StartCoroutine(GenerateWorldWithFixedRadius());
    }

    IEnumerator GenerateWorldWithFixedRadius()
    {
        yield return StartCoroutine(FindDoors());
        GenerateWorld(fixedRadius);
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

        pathTiles = pathGenerator.GetGeneratedPath; // Store path tiles

        // Disable all tiles that are NOT in the path
        foreach (var tile in GeneratedTiles)
        {
            if (!pathTiles.Contains(tile))
            {
                tile.SetActive(false);
            }
        }
    }

    void OnDrawGizmos()
    {
        // Draw the terrain boundary
        Gizmos.color = Color.red;
        Vector3 center = new Vector3((fixedRadius - 1) * 1.5f / 2, 0, (fixedRadius - 1) * 1.5f / 2);
        Vector3 size = new Vector3(fixedRadius * 1.5f, 0.1f, fixedRadius * 1.5f);
        Gizmos.DrawWireCube(center, size);

        // Draw path tiles
        if (pathTiles == null || pathTiles.Count == 0)
            return;

        Gizmos.color = Color.green; // Path tiles color
        foreach (var tile in pathTiles)
        {
            if (tile != null)
            {
                Gizmos.DrawWireCube(tile.transform.position, new Vector3(1.5f, 0.1f, 1.5f));
            }
        }
    }

}
