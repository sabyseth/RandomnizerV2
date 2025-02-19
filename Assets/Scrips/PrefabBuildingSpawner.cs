using UnityEngine;
using UnityEngine.InputSystem;

public class PrefabBuildingSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject buildingPrefab;

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

        if (terrainGenerator != null)
        {
            Vector3 flatSpot = terrainGenerator.GetFlatSpotCenter();
            float spawnY = 0f;

            // Raycast to find exact height
            Vector3 rayOrigin = new Vector3(flatSpot.x, raycastHeight, flatSpot.z);
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                spawnY = hit.point.y;
            }
            else
            {
                Debug.LogWarning("Raycast did not hit ground. Using default Y position.");
            }

            Vector3 spawnPoint = new Vector3(flatSpot.x, spawnY, flatSpot.z);

            if (buildingPrefab != null)
            {
                spawnedBuilding = Instantiate(buildingPrefab, spawnPoint, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Building prefab is not assigned!");
            }
        }
        else
        {
            Debug.LogError("TerrainGenerator not found!");
        }
    }

    void OnDrawGizmos()
    {
        if (terrainGenerator != null)
        {
            Gizmos.color = Color.red;
            Vector3 flatSpot = terrainGenerator.GetFlatSpotCenter();
            Gizmos.DrawWireCube(new Vector3(flatSpot.x, 0, flatSpot.z), new Vector3(20, 1, 20));
        }
    }
}
