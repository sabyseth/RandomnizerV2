using UnityEngine;

public class RandomRoomGenerator : MonoBehaviour
{
    
    public float wallThickness = 0.1f;
    public int minWidth = 1;
    public int maxWidth = 10;
    public int minLength = 1;
    public int maxLength = 10;

    private GameObject parentBox;

    void Start()
    {
        GenerateRandomRoomConfiguration();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateRandomRoomConfiguration();
        }
    }

    void GenerateRandomRoomConfiguration()
    {
        if (parentBox != null)
        {
            Destroy(parentBox);
        }

        parentBox = new GameObject("RandomRoom");

        // Randomly select one of the room configurations
        RoomConfiguration roomConfig = RollForRoomConfiguration();

        // Set up common properties for all room configurations
        roomConfig.wallThickness = wallThickness;
        roomConfig.width = Random.Range(minWidth, maxWidth) * 10;
        roomConfig.length = Random.Range(minLength, maxLength) * 10;
        roomConfig.height = 10; // Default height
        roomConfig.parent = parentBox.transform;

        // Generate the room
        roomConfig.GenerateConfiguration();
    }

    RoomConfiguration RollForRoomConfiguration()
    {
        int roll = Random.Range(0, 6); // Generate a random number between 0 and 5
        switch (roll)
        {
            case 0:
                return parentBox.AddComponent<OneRoom>();
            case 1:
                return parentBox.AddComponent<OneExternalRoom>();
            case 2:
                return parentBox.AddComponent<TwoExternalRooms>();
            case 3:
                return parentBox.AddComponent<OneInternal>();
            case 4:
                return parentBox.AddComponent<TwoInternal>();
            case 5:
                return parentBox.AddComponent<OneExternalOneInternal>();
            default:
                Debug.LogError("Invalid roll");
                return null;
        }
    }
}

public abstract class RoomConfiguration : MonoBehaviour
{
    public float wallThickness = 0.1f;
    public float width = 10;
    public float length = 10;
    public float height = 10;

    public Transform parent;

    public abstract void GenerateConfiguration();

    protected GameObject CreateWall(string name, Vector3 size, Vector3 position, Transform parent)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.localScale = size;
        wall.transform.localPosition = position;
        wall.transform.SetParent(parent);
        wall.GetComponent<Renderer>().material.color = Color.white;
        return wall;
    }

    protected void CreateWallWithDoor(string name, Vector3 size, Vector3 position, Transform parent)
    {
        float doorWidth = 5f;
        float doorHeight = 2f;

        float maxDoorOffset = (size.x / 2) - (doorWidth / 2);
        float doorCenterOffset = Random.Range(-maxDoorOffset, maxDoorOffset);

        float leftWallWidth = (size.x / 2) + doorCenterOffset - (doorWidth / 2);
        float rightWallWidth = (size.x / 2) - doorCenterOffset - (doorWidth / 2);

        CreateWall(name + "_Left", new Vector3(leftWallWidth, size.y, size.z),
            position + new Vector3(-(size.x / 2 - leftWallWidth / 2), 0, 0), parent);

        CreateWall(name + "_Right", new Vector3(rightWallWidth, size.y, size.z),
            position + new Vector3((size.x / 2 - rightWallWidth / 2), 0, 0), parent);

        GameObject doorTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        doorTop.name = name + "_DoorTop";
        doorTop.transform.localScale = new Vector3(doorWidth, doorHeight, size.z);
        doorTop.transform.localPosition = position + new Vector3(doorCenterOffset, (size.y / 2) - (doorHeight / 2), 0);
        doorTop.transform.SetParent(parent);
        doorTop.GetComponent<Renderer>().material.color = Color.white;
    }
}

public class OneRoom : RoomConfiguration
{
    public override void GenerateConfiguration()
    {
        CreateWall("Floor", new Vector3(width + wallThickness, wallThickness, length + wallThickness), 
                   new Vector3(0, -wallThickness / 2, 0), parent);

        CreateWall("Wall_Left", new Vector3(wallThickness, height, length), 
                   new Vector3(-width / 2, height / 2, 0), parent);

        CreateWall("Wall_Right", new Vector3(wallThickness, height, length), 
                   new Vector3(width / 2, height / 2, 0), parent);

        CreateWall("Wall_Back", new Vector3(width, height, wallThickness), 
                   new Vector3(0, height / 2, length / 2), parent);

        CreateWallWithDoor("Wall_Front", new Vector3(width, height, wallThickness), 
                           new Vector3(0, height / 2, -length / 2), parent);
    }

}
public class OneExternalRoom : RoomConfiguration
{
    public override void GenerateConfiguration()
    {
        // Logic for OneExternalRoom goes here.
        Debug.Log("OneExternalRoom configuration");
    }
}

public class TwoExternalRooms : RoomConfiguration
{
    public override void GenerateConfiguration()
    {
        // Logic for TwoExternalRooms goes here.
        Debug.Log("TwoExternalRooms configuration");
    }
}

public class OneInternal : RoomConfiguration
{
    public override void GenerateConfiguration()
    {
        // Logic for OneInternal goes here.
        Debug.Log("OneInternal configuration");
    }
}

public class TwoInternal : RoomConfiguration
{
    public override void GenerateConfiguration()
    {
        // Logic for TwoInternal goes here.
        Debug.Log("TwoInternal configuration");
    }
}

public class OneExternalOneInternal : RoomConfiguration
{
    public override void GenerateConfiguration()
    {
        // Logic for OneExternalOneInternal goes here.
        Debug.Log("OneExternalOneInternal configuration");
    }
}


// Repeat the same structure for OneExternalRoom, TwoExternalRooms, OneInternal, TwoInternal, and OneExternalOneInternal
// Each class overrides GenerateConfiguration() with its unique structure logic.
