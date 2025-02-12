using UnityEngine;
using System.Collections.Generic;

public class CircularPathGenerator : MonoBehaviour
{
    public Terrain terrain;
    public int pathResolution = 100; // Number of points along the path
    public float minRadius = 30f;
    public float maxRadius = 80f;
    public float noiseStrength = 5f; // Randomness in the circular shape

    private List<Vector3> pathPoints = new List<Vector3>();

    void Start()
    {
        GenerateCircularPath();
        DrawPath();
    }

    void GenerateCircularPath()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;

        // Choose a random center within the terrain bounds
        Vector3 center = new Vector3(
            Random.Range(terrainSize.x * 0.3f, terrainSize.x * 0.7f),
            0,
            Random.Range(terrainSize.z * 0.3f, terrainSize.z * 0.7f)
        );

        float radius = Random.Range(minRadius, maxRadius);

        for (int i = 0; i < pathResolution; i++)
        {
            float angle = (i / (float)pathResolution) * Mathf.PI * 2;
            float noisyRadius = radius + Random.Range(-noiseStrength, noiseStrength);

            float x = center.x + Mathf.Cos(angle) * noisyRadius;
            float z = center.z + Mathf.Sin(angle) * noisyRadius;

            // Clamp to terrain bounds
            x = Mathf.Clamp(x, 0, terrainSize.x);
            z = Mathf.Clamp(z, 0, terrainSize.z);

            // Get terrain height at (x, z)
            float y = terrain.SampleHeight(new Vector3(x, 0, z));

            pathPoints.Add(new Vector3(x, y, z));
        }
    }

    void DrawPath()
    {
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Debug.DrawLine(pathPoints[i], pathPoints[i + 1], Color.red, 10f);
        }
    }
}
