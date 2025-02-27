using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path 
{
    private List<GameObject> path = new List<GameObject>();
    private List<GameObject> doorTiles = new List<GameObject>();
    private int radius;
    private List<GameObject> allTiles;

    public List<GameObject> GetGeneratedPath => path;

    public Path(int worldRadius, List<GameObject> allTiles)
    {
        this.radius = worldRadius;
        this.allTiles = allTiles;
    }

    public void AssignDoorTiles(List<Vector3> doorPositions)
    {
        foreach (var pos in doorPositions)
        {
            GameObject nearestTile = FindNearestTile(pos);
            if (nearestTile != null)
            {
                doorTiles.Add(nearestTile);
            }
            else
            {
                Debug.LogWarning($"No valid tile found near door at {pos}");
            }
        }
    }


    private GameObject FindNearestTile(Vector3 position)
    {
        GameObject nearestTile = null;
        float minDistance = float.MaxValue;

        foreach (var tile in allTiles)
        {
            float distance = Vector3.Distance(tile.transform.position, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTile = tile;
            }
        }
        return nearestTile;
    }

    public void GeneratePath()
    {
        if (doorTiles.Count < 2)
            return;

        for (int i = 0; i < doorTiles.Count - 1; i++)
        {
            ConnectTiles(doorTiles[i], doorTiles[i + 1]);
        }
    }

    private void ConnectTiles(GameObject start, GameObject end)
    {
        GameObject currentTile = start;
        HashSet<GameObject> visitedTiles = new HashSet<GameObject>();

        while (currentTile != end)
        {
            // Avoid infinite loops
            if (visitedTiles.Contains(currentTile))
            {
                Debug.LogError("Infinite loop detected in ConnectTiles. Breaking out.");
                break;
            }
            visitedTiles.Add(currentTile);

            path.Add(currentTile);

            if (Mathf.Abs(currentTile.transform.position.x - end.transform.position.x) > 
                Mathf.Abs(currentTile.transform.position.z - end.transform.position.z))
            {
                if (currentTile.transform.position.x > end.transform.position.x)
                    MoveLeft(ref currentTile);
                else
                    MoveRight(ref currentTile);
            }
            else
            {
                if (currentTile.transform.position.z > end.transform.position.z)
                    MoveDown(ref currentTile);
                else
                    MoveUp(ref currentTile);
            }
        }
        path.Add(end);
    }



    private void MoveDown(ref GameObject currentTile) 
    {
        currentTile = GetTileAtOffset(currentTile, -radius);
    }

    private void MoveUp(ref GameObject currentTile) 
    {
        currentTile = GetTileAtOffset(currentTile, radius);
    }
    
    private void MoveLeft(ref GameObject currentTile) 
    {
        currentTile = GetTileAtOffset(currentTile, -1);
    }
    
    private void MoveRight(ref GameObject currentTile)
    {
        currentTile = GetTileAtOffset(currentTile, 1);
    }

    private GameObject GetTileAtOffset(GameObject currentTile, int offset)
    {
        int index = WorldGeneration.GeneratedTiles.IndexOf(currentTile);
        if (index == -1)
        {
            Debug.LogError("Tile not found in GeneratedTiles.");
            return currentTile; // Prevent crashes
        }

        int newIndex = index + offset;
        if (newIndex >= 0 && newIndex < WorldGeneration.GeneratedTiles.Count)
        {
            return WorldGeneration.GeneratedTiles[newIndex];
        }

        Debug.LogWarning("Attempted to access out-of-bounds tile.");
        return currentTile; // Prevent invalid movement
    }

}
