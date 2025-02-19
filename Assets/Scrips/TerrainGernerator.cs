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
    private Vector2 flatSpotPosition;

    private Terrain terrain;

    void Start()
    {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        treeGeneration = GetComponent<TreeGeneration>();
        terrain = GetComponent<Terrain>();

        // Pick a random spot for the flat area
        flatSpotPosition = new Vector2(Random.Range(50, width - 50), Random.Range(50, height - 50));

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
                // Normal terrain generation
                float heightValue = Mathf.PerlinNoise((x + offsetX) / scale, (y + offsetY) / scale);

                // Flatten a specific area
                if (x > flatSpotPosition.x - flatSpotSize.x / 2 && x < flatSpotPosition.x + flatSpotSize.x / 2 &&
                    y > flatSpotPosition.y - flatSpotSize.y / 2 && y < flatSpotPosition.y + flatSpotSize.y / 2)
                {
                    heightValue = 0.2f; // Lower flat height value
                }

                heights[x, y] = heightValue;
            }
        }

        return heights;
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

    public Vector3 GetFlatSpotCenter()
    {
        return new Vector3(flatSpotPosition.x, 0, flatSpotPosition.y);
    }
}
