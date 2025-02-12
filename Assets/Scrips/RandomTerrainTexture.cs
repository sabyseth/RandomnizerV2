using UnityEngine;

public class RandomTerrainTexture : MonoBehaviour
{
    public Terrain terrain;
    private TerrainData terrainData;

    void Start()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();

        terrainData = terrain.terrainData;

        ApplyRandomTexture();
    }

    void ApplyRandomTexture()
    {
        int width = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;

        float[,,] splatmapData = new float[height, width, terrainData.alphamapLayers];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int randomTexture = Random.Range(0, 3); // Choose one of the three textures
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    splatmapData[y, x, i] = (i == randomTexture) ? 1.0f : 0.0f;
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }
}
