using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PrefabBuildingSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject buildingPrefab;

    [Header("Spawn Settings")]
    public int minBuildings = 3;
    public int maxBuildings = 10;

    [Header("Spawn Position Range")]
    public Vector3 minSpawnPosition = new Vector3(-10, 0, -10);
    public Vector3 maxSpawnPosition = new Vector3(10, 0, 10);

    [Header("Raycast Settings")]
    public LayerMask groundLayer;
    public float raycastHeight = 10f;
    public float flattenRadius = 5f;
    public float flatSpotHeight = 1f; // New variable to adjust the height of the flat spot

    private Terrain terrain;
    private List<GameObject> spawnedBuildings = new List<GameObject>();

    void Start()
    {
        GameObject terrainObject = GameObject.FindGameObjectWithTag("Terrain");
        if (terrainObject != null)
        {
            terrain = terrainObject.GetComponent<Terrain>();
        }

        if (terrain == null)
        {
            Debug.LogError("Terrain not found! Ensure it has the correct tag and a Terrain component.");
        }
        else
        {
            SpawnBuildings();
        }
    }

    void Update()
    {
    #if UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            SpawnBuildings();
        }
    #endif
    }

    void SpawnBuildings()
    {
        foreach (var building in spawnedBuildings)
        {
            Destroy(building);
        }
        spawnedBuildings.Clear();

        int buildingCount = Random.Range(minBuildings, maxBuildings + 1);
        float minBuildingDistance = 5f; // Set a minimum distance between buildings

        for (int i = 0; i < buildingCount; i++)
        {
            int maxAttempts = 10; // Prevent infinite loops
            int attempt = 0;
            bool validPosition = false;
            Vector3 spawnPoint = Vector3.zero;

            while (attempt < maxAttempts && !validPosition)
            {
                float randomX = Random.Range(minSpawnPosition.x, maxSpawnPosition.x);
                float randomZ = Random.Range(minSpawnPosition.z, maxSpawnPosition.z);
                float spawnY = 0f;

                Vector3 rayOrigin = new Vector3(randomX, raycastHeight, randomZ);
                RaycastHit hit;
                if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, groundLayer))
                {
                    spawnY = hit.point.y + flatSpotHeight;
                    spawnPoint = new Vector3(randomX, spawnY, randomZ);

                    // Check distance to all previously spawned buildings
                    validPosition = true;
                    foreach (var existingBuilding in spawnedBuildings)
                    {
                        if (Vector3.Distance(spawnPoint, existingBuilding.transform.position) < minBuildingDistance)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                }
                attempt++;
            }

            if (validPosition && buildingPrefab != null)
            {
                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                GameObject newBuilding = Instantiate(buildingPrefab, spawnPoint, randomRotation);
                spawnedBuildings.Add(newBuilding);
            }
        }
    }


    void AdjustTerrain(Vector3 position, float targetHeight, Quaternion rotation, float buildingSize)
    {
        if (terrain == null)
        {
            Debug.LogError("No terrain found!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPosition = terrain.transform.position;

        int mapX = Mathf.RoundToInt((position.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
        int mapZ = Mathf.RoundToInt((position.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);
        int radius = Mathf.RoundToInt(flattenRadius / terrainData.size.x * terrainData.heightmapResolution);

        if (mapX - radius < 0 || mapZ - radius < 0 || mapX + radius >= terrainData.heightmapResolution || mapZ + radius >= terrainData.heightmapResolution)
        {
            Debug.LogWarning("Adjustment area is out of terrain bounds.");
            return;
        }

        float[,] heights = terrainData.GetHeights(mapX - radius, mapZ - radius, radius * 2, radius * 2);
        float normalizedHeight = (targetHeight + flatSpotHeight) / terrainData.size.y; // Apply height adjustment

        Vector3 forward = rotation * Vector3.forward;
        Vector3 right = rotation * Vector3.right;

        for (int x = 0; x < radius * 2; x++)
        {
            for (int z = 0; z < radius * 2; z++)
            {
                Vector3 worldPos = new Vector3(position.x + (x - radius), position.y, position.z + (z - radius));

                // Project points onto the expected plane based on building rotation
                Vector3 localOffset = (x - radius) * right + (z - radius) * forward;
                Vector3 adjustedPoint = position + localOffset;

                // Raycast to find the terrain height at this adjusted point
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(adjustedPoint.x, position.y + 10f, adjustedPoint.z), Vector3.down, out hit, Mathf.Infinity, groundLayer))
                {
                    heights[x, z] = (hit.point.y + flatSpotHeight) / terrainData.size.y; // Apply height adjustment
                }
                else
                {
                    heights[x, z] = normalizedHeight;
                }
            }
        }

        terrainData.SetHeights(mapX - radius, mapZ - radius, heights);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 spawnAreaSize = maxSpawnPosition - minSpawnPosition;
        Vector3 spawnAreaCenter = (maxSpawnPosition + minSpawnPosition) / 2;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
