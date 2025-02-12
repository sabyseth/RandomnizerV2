using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    private int[] squareSizes;
    public int numberOfSquares = 1;
    public int minSize = 2;
    public int maxSize = 10;

    void Start()
    {
        WallGenerator();
    }

    string GenerateSquare(int size)
    {
        string squareRepresentation = "";

        // Adjust these to match your prefab dimensions
        float wallPrefabWidth = 10f; 
        float wallPrefabHeight = 10f; 
        float floorPrefabWidth = 10f; 
        float floorPrefabHeight = 10f;

        // Generate the floor
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                // Place the floor tile
                Vector3 floorPosition = new Vector3(col * floorPrefabWidth, 0f, row * floorPrefabHeight);
                Instantiate(floorPrefab, floorPosition, Quaternion.identity);
                squareRepresentation += "O";
            }
            squareRepresentation += "\n";
        }

        // Generate the walls
        for (int i = 0; i < size; i++)
        {
            // Left wall
            Vector3 leftWallPosition = new Vector3(-4.85f, (wallPrefabHeight / 2) + .15f, i * wallPrefabWidth);
            Instantiate(wallPrefab, leftWallPosition, Quaternion.identity);

            // Right wall
            Vector3 rightWallPosition = new Vector3((size - 1) * floorPrefabWidth + 4.85f, (wallPrefabHeight / 2) + .15f, i * wallPrefabWidth);
            Instantiate(wallPrefab, rightWallPosition, Quaternion.identity);
        }

        for (int j = 0; j < size; j++)
        {
            // Bottom wall
            Vector3 bottomWallPosition = new Vector3(j * floorPrefabWidth, wallPrefabHeight / 2, 0f);
            Instantiate(wallPrefab, bottomWallPosition, Quaternion.Euler(0f, 90f, 0f));

            // Top wall
            Vector3 topWallPosition = new Vector3(j * floorPrefabWidth, wallPrefabHeight / 2, (size - 1) * floorPrefabHeight);
            Instantiate(wallPrefab, topWallPosition, Quaternion.Euler(0f, 90f, 0f));
        }

        return squareRepresentation;
    }

    public void WallGenerator()
    {
        squareSizes = new int[numberOfSquares];

        for (int i = 0; i < numberOfSquares; i++)
        {
            int size = Random.Range(minSize, maxSize);
            squareSizes[i] = size;
            Debug.Log($"Square {i + 1}:\n" + GenerateSquare(size));
        }
    }
}
