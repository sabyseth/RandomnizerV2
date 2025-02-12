using UnityEngine;
using UnityEngine.InputSystem;

public class PrefabBuildingSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject buildingPrefab;

    [Header("Spawn Settings")]
    public Vector3 spawnPoint = Vector3.zero;

    [Header("Spawn Position Range")]
    public Vector3 minSpawnPosition = new Vector3(-10, 0, -10);
    public Vector3 maxSpawnPosition = new Vector3(10, 0, 10);

    [Header("Raycast Settings")]
    public LayerMask groundLayer;
    public float raycastHeight = 10f;

    private GameObject spawnedBuilding;
    private TerrainGenerator terrainGenerator;

    void Start()
    {
        terrainGenerator = FindObjectOfType<TerrainGenerator>();
        SpawnBuilding();
    }

    void Update()
    {
    #if UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            SpawnBuilding();
        }
    #endif
    }

    void SpawnBuilding()
    {
        if (spawnedBuilding != null)
        {
            Destroy(spawnedBuilding);
        }

        float randomX = Random.Range(minSpawnPosition.x, maxSpawnPosition.x);
        float randomZ = Random.Range(minSpawnPosition.z, maxSpawnPosition.z);
        float spawnY = 0f;

        Vector3 rayOrigin = new Vector3(randomX, raycastHeight, randomZ);
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            spawnY = hit.point.y;
        }
        else
        {
            Debug.LogWarning("Raycast did not hit ground. Using default Y position.");
        }

        spawnPoint = new Vector3(randomX, spawnY, randomZ);

        if (buildingPrefab != null)
        {
            spawnedBuilding = Instantiate(buildingPrefab, spawnPoint, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Building prefab is not assigned!");
        }

        if (terrainGenerator != null)
        {
            terrainGenerator.SetFlatSpot(spawnPoint);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 spawnAreaSize = maxSpawnPosition - minSpawnPosition;
        Vector3 spawnAreaCenter = (maxSpawnPosition + minSpawnPosition) / 2;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
