using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int depth = 20;
    public int width = 256;
    public int height = 256;
    public float scale = 20f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    public Material[] terrainMaterials;
    private TreeGeneration treeGeneration;

    [Header("Flat Spot Settings")]
    public Vector2 flatSpotSize = new Vector2(20, 20);

    private Terrain terrain;
    private Vector2 flatSpotPosition;

    void Start()
    {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
        treeGeneration = GetComponent<TreeGeneration>();
        terrain = GetComponent<Terrain>();
        GenerateTerrainWithMaterial();
    }

    public void SetFlatSpot(Vector3 buildingWorldPosition)
    {
        Debug.LogError(buildingWorldPosition.x);
        if (terrain == null) return;

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;

        // Convert world position to terrain heightmap position
        float normalizedX = (buildingWorldPosition.x - terrain.transform.position.x) / terrainSize.x;
        float normalizedZ = (buildingWorldPosition.z - terrain.transform.position.z) / terrainSize.z;

        flatSpotPosition = new Vector2(normalizedX * width, normalizedZ * height);

        GenerateTerrainWithMaterial();
    }

    void GenerateTerrainWithMaterial()
    {
        if (terrain != null)
        {
            terrain.terrainData = GenerateTerrain(terrain.terrainData);
            AssignRandomMaterial();
        }
        else
        {
            Debug.LogError("Terrain component not found on this GameObject.");
        }
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeightWithFlatSpot(x, y);
            }
        }
        return heights;
    }

    float CalculateHeightWithFlatSpot(int x, int y)
    {
        if (x >= flatSpotPosition.x - flatSpotSize.x / 2 && x <= flatSpotPosition.x + flatSpotSize.x / 2 &&
            y >= flatSpotPosition.y - flatSpotSize.y / 2 && y <= flatSpotPosition.y + flatSpotSize.y / 2)
        {
            return 0.5f / depth;
        }

        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;
        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    void AssignRandomMaterial()
    {
        if (terrainMaterials.Length > 0)
        {
            int randomIndex = Random.Range(0, terrainMaterials.Length);
            terrain.materialTemplate = terrainMaterials[randomIndex];
            treeGeneration.SetTerrainMaterial(terrainMaterials[randomIndex]);
        }
        else
        {
            Debug.LogError("No materials assigned to terrainMaterials array.");
        }
    }
}
