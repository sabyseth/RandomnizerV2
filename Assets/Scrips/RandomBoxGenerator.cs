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

    protected void CreateWallWithDoor(string name, Vector3 size, Vector3 position, Transform parent, Vector3 direction)
    {
        float doorWidth = 5f;
        float doorHeight = 2f;

        if (direction == Vector3.forward || direction == Vector3.back)
        {
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
        else if (direction == Vector3.right || direction == Vector3.left)
        {
            float maxDoorOffset = (size.z / 2) - (doorWidth / 2);
            float doorCenterOffset = Random.Range(-maxDoorOffset, maxDoorOffset);

            float leftWallDepth = (size.z / 2) + doorCenterOffset - (doorWidth / 2);
            float rightWallDepth = (size.z / 2) - doorCenterOffset - (doorWidth / 2);

            CreateWall(name + "_Left", new Vector3(size.x, size.y, leftWallDepth),
                position + new Vector3(0, 0, -(size.z / 2 - leftWallDepth / 2)), parent);

            CreateWall(name + "_Right", new Vector3(size.x, size.y, rightWallDepth),
                position + new Vector3(0, 0, (size.z / 2 - rightWallDepth / 2)), parent);

            GameObject doorTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            doorTop.name = name + "_DoorTop";
            doorTop.transform.localScale = new Vector3(size.x, doorHeight, doorWidth);
            doorTop.transform.localPosition = position + new Vector3(0, (size.y / 2) - (doorHeight / 2), doorCenterOffset);
            doorTop.transform.SetParent(parent);
            doorTop.GetComponent<Renderer>().material.color = Color.white;
        }
    }

}
public class OneRoom : RoomConfiguration
{
    public override void GenerateConfiguration()
    {
        Debug.Log("OneRoom configuration");
        CreateWall("Floor", new Vector3(width + wallThickness, wallThickness, length + wallThickness), 
                   new Vector3(0, -wallThickness / 2, 0), parent);

        CreateWall("Wall_Left", new Vector3(wallThickness, height, length), 
                   new Vector3(-width / 2, height / 2, 0), parent);

        CreateWall("Wall_Right", new Vector3(wallThickness, height, length), 
                   new Vector3(width / 2, height / 2, 0), parent);

        CreateWall("Wall_Back", new Vector3(width, height, wallThickness), 
                   new Vector3(0, height / 2, length / 2), parent);

        //CreateWallWithDoor("Wall_Front", new Vector3(width, height, wallThickness), 
          //                 new Vector3(0, height / 2, -length / 2), parent);
    }

}

public class OneExternalRoom : RoomConfiguration
{
    public override void GenerateConfiguration()
    {
         Debug.Log("OneExternalRoom configuration");

    }
}

public class TwoExternalRooms : RoomConfiguration
{
    public float internalWallThickness = 0.3f; // Thicker middle wall

    public override void GenerateConfiguration()
    {
        Debug.Log("TwoExternalRooms configuration");

        // Randomize the second room size (between 50% and 80% of the first room)
        float primaryRoomWidth = width * Random.Range(0.5f, 0.8f);
        float secondaryRoomWidth = width - primaryRoomWidth;
        float roomHeight = height;
        float roomLength = length;

        // Create first (larger) room
        Transform room1 = new GameObject("Room1").transform;
        room1.SetParent(parent);
        CreateRoom(room1, new Vector3(-width / 2 + primaryRoomWidth / 2, 0, 0), primaryRoomWidth, roomHeight, roomLength, true);

        // Create second (smaller) room
        Transform room2 = new GameObject("Room2").transform;
        room2.SetParent(parent);
        CreateRoom(room2, new Vector3(width / 2 - secondaryRoomWidth / 2, 0, 0), secondaryRoomWidth, roomHeight, roomLength, true);

        // Create the thicker shared internal wall with a door
        CreateWallWithDoor("SharedWall", new Vector3(internalWallThickness, roomHeight, roomLength),
            new Vector3(-width / 2 + primaryRoomWidth + (internalWallThickness / 2), roomHeight / 2, 0), parent, Vector3.right);
    }

    void CreateRoom(Transform roomParent, Vector3 position, float roomWidth, float roomHeight, float roomLength, bool hasExternalDoor)
    {
        CreateWall("Floor", new Vector3(roomWidth, wallThickness, roomLength), 
                   position + new Vector3(0, -wallThickness / 2, 0), roomParent);
        CreateWall("Wall_Left", new Vector3(wallThickness, roomHeight, roomLength), 
                   position + new Vector3(-roomWidth / 2, roomHeight / 2, 0), roomParent);
        CreateWall("Wall_Right", new Vector3(wallThickness, roomHeight, roomLength), 
                   position + new Vector3(roomWidth / 2, roomHeight / 2, 0), roomParent);
        CreateWall("Wall_Back", new Vector3(roomWidth, roomHeight, wallThickness), 
                   position + new Vector3(0, roomHeight / 2, roomLength / 2), roomParent);

        if (hasExternalDoor)
        {
            CreateWallWithDoor("Wall_Front", new Vector3(roomWidth, roomHeight, wallThickness),
                position + new Vector3(0, roomHeight / 2, -roomLength / 2), roomParent, Vector3.forward);
        }
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
