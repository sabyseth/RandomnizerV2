using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public Terrain terrain;
    public int pathWidth = 3;
    public int pathLength = 100;
    public float smoothness = 0.2f;
    public List<GameObject> obstacles;
    public Texture2D pathTexture;
    
    private int terrainWidth;
    private int terrainHeight;
    private float[,] heights;
    private TerrainLayer[] terrainLayers;

    void Start()
    {
        if (terrain == null)
        {
            terrain = GetComponent<Terrain>();
        }
        
        terrainWidth = terrain.terrainData.heightmapResolution;
        terrainHeight = terrain.terrainData.heightmapResolution;
        heights = terrain.terrainData.GetHeights(0, 0, terrainWidth, terrainHeight);
        terrainLayers = terrain.terrainData.terrainLayers;
        
        GeneratePath();
    }

    void GeneratePath()
    {
        int startX = Random.Range(10, terrainWidth - 10);
        int startZ = 0;
        
        for (int i = 0; i < pathLength; i++)
        {
            int x = Mathf.Clamp(startX + Random.Range(-1, 2), pathWidth, terrainWidth - pathWidth);
            int z = Mathf.Clamp(startZ + 1, 0, terrainHeight - 1);
            
            ClearPath(x, z);
            ChangePathColor(x, z);
            RemoveObstacles(new Vector3(x, terrain.SampleHeight(new Vector3(x, 0, z)), z));
            startX = x;
            startZ = z;
        }
        
        terrain.terrainData.SetHeights(0, 0, heights);
    }

    void ClearPath(int x, int z)
    {
        for (int i = -pathWidth / 2; i <= pathWidth / 2; i++)
        {
            for (int j = -pathWidth / 2; j <= pathWidth / 2; j++)
            {
                int newX = Mathf.Clamp(x + i, 0, terrainWidth - 1);
                int newZ = Mathf.Clamp(z + j, 0, terrainHeight - 1);
                heights[newX, newZ] = Mathf.Lerp(heights[newX, newZ], 0f, smoothness);
            }
        }
    }

    void ChangePathColor(int x, int z)
    {
        TerrainLayer pathLayer = new TerrainLayer();
        pathLayer.diffuseTexture = pathTexture;
        terrainLayers[0] = pathLayer;
        terrain.terrainData.terrainLayers = terrainLayers;
    }

    void RemoveObstacles(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, pathWidth);
        foreach (Collider col in colliders)
        {
            if (obstacles.Contains(col.gameObject))
            {
                Destroy(col.gameObject);
            }
        }
    }
}
