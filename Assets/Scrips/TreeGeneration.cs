using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGeneration : MonoBehaviour
{
    public int width;
    public int height;
    public float scale;
    public float TreeScaleMin; 
    public float TreeScaleMax; 
    public GameObject[] treePrefabs;
    public GameObject[] cactusPrefabs;  // Array for cacti (for desert)
    public GameObject[] snowPrefab;  // Array for snow-related objects
    public GameObject[] randomObjects; // Array for additional random objects
    public float acceptancePoint; 
    public float heightOffset = 0.1f; 
    public float minObjectScale = 0.5f;
    public float maxObjectScale = 2.0f;
    public float objectSpawnChance = 0.5f; // Probability of an object spawning
    public int minRandomObjects = 5; // Minimum number of random objects to spawn
    public int maxRandomObjects = 15; // Maximum number of random objects to spaw
    private Material currentTerrainMaterial;  // To track the selected terrain material

    void Start()
    {
        LoadPrefabsFromResources();
        scale = Random.Range(TreeScaleMin, TreeScaleMax);  
        acceptancePoint = Random.Range(0.8f, 0.9f);
        
        Invoke("SpawnAssets", 1);
    }

    void LoadPrefabsFromResources()
    {
        treePrefabs = Resources.LoadAll<GameObject>("Trees");
        cactusPrefabs = Resources.LoadAll<GameObject>("Cacti");
        snowPrefab = Resources.LoadAll<GameObject>("Snow");
        randomObjects = Resources.LoadAll<GameObject>("RandomObjects");

        Debug.Log($"Loaded {treePrefabs.Length} tree prefabs, {cactusPrefabs.Length} cacti, {snowPrefab.Length} snow objects, and {randomObjects.Length} random objects.");
    }

    public void SetTerrainMaterial(Material terrainMaterial)
    {
        currentTerrainMaterial = terrainMaterial;
    }

    public void SpawnAssets()
    {
        for (int y = 0; y < height; y++) 
        {
            for (int x = 0; x < width; x++)
            {
                float xValue = x * scale;
                float yValue = y * scale;

                float perlinValue = Mathf.PerlinNoise(xValue, yValue);

                if (perlinValue >= acceptancePoint)
                {
                    Debug.Log($"Running FindLand for position: {x}, {y} with Perlin value: {perlinValue}");
                    GameObject selectedPrefab = SelectPrefabBasedOnTerrain();
                    FindLand(new Vector3(x, 0, y), selectedPrefab, false, true);
                }
            }
        }

        SpawnRandomObjects();
    }

    public void FindLand(Vector3 position, GameObject Prefab, bool randomizeTransform, bool scaleByTen)
    {
        if (Prefab == null || Random.value > objectSpawnChance) return;

        Ray ray = new Ray(position + Vector3.up * 10f, Vector3.down);        
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.Log($"Land found at {hitInfo.point}. Spawning asset.");
            GameObject obj = Instantiate(Prefab, hitInfo.point, randomizeTransform ? RandomRotation() : Quaternion.Euler(-90f, 0f, 0f));
            
            if (randomizeTransform)
            {
                float randomScale = Random.Range(minObjectScale, maxObjectScale);
                obj.transform.localScale *= randomScale;
            }
            else if (scaleByTen)
            {
                obj.transform.localScale *= 7f;
            }

            // Ensure a MeshCollider is attached
            MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = obj.AddComponent<MeshCollider>();
            }
            
            // Check if the object is touching the terrain after instantiation
            if (!IsTouchingTerrain(obj))
            {
                Debug.LogWarning($"Spawned object at {hitInfo.point} is not touching the terrain. Destroying it.");
                Destroy(obj);
            }
        }
        else
        {
            Debug.LogWarning($"No land found under position {position}. Object won't be spawned.");
        }
    }

    private bool IsTouchingTerrain(GameObject obj)
    {
        Collider[] colliders = Physics.OverlapSphere(obj.transform.position, 0.5f);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("Terrain")) // Make sure your terrain has the "Terrain" tag
            {
                return true;
            }
        }
        return false;
    }



    // Generate a random rotation on all axes
    Quaternion RandomRotation()
    {
        return Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }

    // Spawn random objects at random positions
    void SpawnRandomObjects()
    {
        if (randomObjects.Length == 0) return;
        
        int randomCount = Random.Range(minRandomObjects, maxRandomObjects); // Use min/max values for number of objects
        for (int i = 0; i < randomCount; i++)
        {
            if (Random.value > objectSpawnChance) continue;
            
            Vector3 randomPos = new Vector3(Random.Range(0, width), 0, Random.Range(0, height));
            GameObject randomPrefab = randomObjects[Random.Range(0, randomObjects.Length)];
            FindLand(randomPos, randomPrefab, true, false);
        }
    }

    // Select prefab based on the current terrain material
    GameObject SelectPrefabBasedOnTerrain()
    {
        if (currentTerrainMaterial != null)
        {
            string materialName = currentTerrainMaterial.name.ToLower();
            Debug.Log("Selected Terrain Material: " + materialName);  // Debug log to check the material name

            if (materialName.Contains("desert"))
            {
                if (cactusPrefabs.Length > 0)
                    return cactusPrefabs[Random.Range(0, cactusPrefabs.Length)];
                else
                    Debug.LogWarning("No cactus prefabs assigned.");
            }
            else if (materialName.Contains("snow"))
            {
                if (snowPrefab.Length > 0)
                    return snowPrefab[Random.Range(0, snowPrefab.Length)];
                else
                    Debug.LogWarning("No snow prefabs assigned.");
            }
            else if (materialName.Contains("woods"))
            {
                if (treePrefabs.Length > 0)
                    return treePrefabs[Random.Range(0, treePrefabs.Length)];
                else
                    Debug.LogWarning("No tree prefabs assigned.");
            }
        }
        else
        {
            Debug.LogError("Terrain material is null.");
        }

        return null;  // Return null if no prefab was selected
    }
    
}
