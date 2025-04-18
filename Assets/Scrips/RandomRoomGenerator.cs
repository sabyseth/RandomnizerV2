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
        roomConfig.height = 5; // Default height
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
                return parentBox.AddComponent<OneRoomDevided>();
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
    public float width = 5;
    public float length = 5;
    public float height = 5;

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
        float doorWidth = 3f;
        float doorHeight = 1f;

        if (direction == Vector3.forward || direction == Vector3.back)
        {
            float maxDoorOffset = (size.x / 2) - (doorWidth / 2);
            float doorCenterOffset = Random.Range(-maxDoorOffset, maxDoorOffset);

            float leftWallWidth = (size.x / 2) + doorCenterOffset - (doorWidth / 2);
            float rightWallWidth = (size.x / 2) - doorCenterOffset - (doorWidth / 2);

            // Create left and right walls
            CreateWall(name + "_Left", new Vector3(leftWallWidth, size.y, size.z),
                position + new Vector3(-(size.x / 2 - leftWallWidth / 2), 0, 0), parent);

            CreateWall(name + "_Right", new Vector3(rightWallWidth, size.y, size.z),
                position + new Vector3((size.x / 2 - rightWallWidth / 2), 0, 0), parent);

            // Create door
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = name + "_Door";
            door.transform.localScale = new Vector3(doorWidth, doorHeight, size.z);
            door.transform.localPosition = position + new Vector3(doorCenterOffset, (size.y / 2) - (doorHeight / 2), 0);
            door.transform.SetParent(parent);
            door.GetComponent<Renderer>().material.color = Color.white;
        }
        else if (direction == Vector3.right || direction == Vector3.left)
        {
            float maxDoorOffset = (size.z / 2) - (doorWidth / 2);
            float doorCenterOffset = Random.Range(-maxDoorOffset, maxDoorOffset);

            float leftWallDepth = (size.z / 2) + doorCenterOffset - (doorWidth / 2);
            float rightWallDepth = (size.z / 2) - doorCenterOffset - (doorWidth / 2);

            // Create left and right walls
            CreateWall(name + "_Left", new Vector3(size.x, size.y, leftWallDepth),
                position + new Vector3(0, 0, -(size.z / 2 - leftWallDepth / 2)), parent);

            CreateWall(name + "_Right", new Vector3(size.x, size.y, rightWallDepth),
                position + new Vector3(0, 0, (size.z / 2 - rightWallDepth / 2)), parent);

            // Create door
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = name + "_Door";
            door.transform.localScale = new Vector3(size.x, doorHeight, doorWidth);
            door.transform.localPosition = position + new Vector3(0, (size.y / 2) - (doorHeight / 2), doorCenterOffset);
            door.transform.SetParent(parent);
            door.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    protected void createBaseRoom()
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
                        new Vector3(0, height / 2, -length / 2), parent, Vector3.back);
    }

}

public class OneRoom : RoomConfiguration
{
    public override void GenerateConfiguration()
    {
        Debug.Log("OneRoom configuration");
        createBaseRoom();
    }

}

public class OneRoomDevided : RoomConfiguration
{
    // Declare variables at the class level
    public float wallThickness = 0.1f;
    public float width = 10;
    public float length = 10;
    public float height = 10;

    public Transform parent;

    // Override the GenerateConfiguration method
    public override void GenerateConfiguration()
    {
        // Create the base room
        createBaseRoom();
    }

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

            // Create left and right walls
            CreateWall(name + "_Left", new Vector3(leftWallWidth, size.y, size.z),
                position + new Vector3(-(size.x / 2 - leftWallWidth / 2), 0, 0), parent);

            CreateWall(name + "_Right", new Vector3(rightWallWidth, size.y, size.z),
                position + new Vector3((size.x / 2 - rightWallWidth / 2), 0, 0), parent);

            // Create door
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = name + "_Door";
            door.transform.localScale = new Vector3(doorWidth, doorHeight, size.z);
            door.transform.localPosition = position + new Vector3(doorCenterOffset, (size.y / 2) - (doorHeight / 2), 0);
            door.transform.SetParent(parent);
            door.GetComponent<Renderer>().material.color = Color.white;
        }
        else if (direction == Vector3.right || direction == Vector3.left)
        {
            float maxDoorOffset = (size.z / 2) - (doorWidth / 2);
            float doorCenterOffset = Random.Range(-maxDoorOffset, maxDoorOffset);

            float leftWallDepth = (size.z / 2) + doorCenterOffset - (doorWidth / 2);
            float rightWallDepth = (size.z / 2) - doorCenterOffset - (doorWidth / 2);

            // Create left and right walls
            CreateWall(name + "_Left", new Vector3(size.x, size.y, leftWallDepth),
                position + new Vector3(0, 0, -(size.z / 2 - leftWallDepth / 2)), parent);

            CreateWall(name + "_Right", new Vector3(size.x, size.y, rightWallDepth),
                position + new Vector3(0, 0, (size.z / 2 - rightWallDepth / 2)), parent);

            // Create door
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = name + "_Door";
            door.transform.localScale = new Vector3(size.x, doorHeight, doorWidth);
            door.transform.localPosition = position + new Vector3(0, (size.y / 2) - (doorHeight / 2), doorCenterOffset);
            door.transform.SetParent(parent);
            door.GetComponent<Renderer>().material.color = Color.white;
        }
    }


    protected void CreateRandomDividingWallWithDoor()
    {

        bool isVertical = Random.Range(0, 2) == 0; 
        Vector3 wallPosition = Vector3.zero;


        if (isVertical)
        {

            wallPosition = new Vector3(Random.Range(-width / 2 + wallThickness, width / 2 - wallThickness), height / 2, 0);
            CreateWallWithDoor("DividingWall_Vertical", new Vector3(wallThickness, height, length), wallPosition, parent, Vector3.right);
        }
        else
        {
            // Horizontal wall: divide the room in half horizontally
            wallPosition = new Vector3(0, height / 2, Random.Range(-length / 2 + wallThickness, length / 2 - wallThickness));
            CreateWallWithDoor("DividingWall_Horizontal", new Vector3(width, height, wallThickness), wallPosition, parent, Vector3.forward);
        }
    }

    // Function to create the base room layout
    protected void createBaseRoom()
    {
        // Create the floor
        CreateWall("Floor", new Vector3(width + wallThickness, wallThickness, length + wallThickness), 
                    new Vector3(0, -wallThickness / 2, 0), parent);

        // Create the left wall
        CreateWall("Wall_Left", new Vector3(wallThickness, height, length), 
                    new Vector3(-width / 2, height / 2, 0), parent);

        // Create the right wall
        CreateWall("Wall_Right", new Vector3(wallThickness, height, length), 
                    new Vector3(width / 2, height / 2, 0), parent);

        // Create the back wall
        CreateWall("Wall_Back", new Vector3(width, height, wallThickness), 
                    new Vector3(0, height / 2, length / 2), parent);

        // Create the front wall with a door
        CreateWallWithDoor("Wall_Front", new Vector3(width, height, wallThickness), 
                            new Vector3(0, height / 2, -length / 2), parent, Vector3.back);

        // Create a random dividing wall with a door
        CreateRandomDividingWallWithDoor();
    }
}



public class TwoExternalRooms : RoomConfiguration
{
    public override void GenerateConfiguration()
    {
        // Logic for TwoInternal goes here.
        Debug.Log("TwoInternal configuration");
    }
}

public class OneInternal : RoomConfiguration
{
    public float internalWallThickness = 0.3f; // Thicker middle wall

    public override void GenerateConfiguration()
    {
        Debug.Log("TwoExternalRooms configuration left");

        // Randomize the room sizes
        float primaryRoomWidth = width * Random.Range(0.5f, 0.8f);
        float secondaryRoomWidth = width - primaryRoomWidth;
        float roomHeight = height;

        // Make sure the lengths are different by at least 20%
        float primaryRoomLength = length;
        float secondaryRoomLength;
        do {
            secondaryRoomLength = length * Random.Range(0.5f, 0.9f);
        } while (Mathf.Abs(primaryRoomLength - secondaryRoomLength) < length * 0.2f);

        // Create first room (now on the right)
        Transform room1 = new GameObject("Room1").transform;
        room1.SetParent(parent);
        CreateRoom(room1, new Vector3(width / 2 - primaryRoomWidth / 2, 0, 0), 
                primaryRoomWidth, roomHeight, primaryRoomLength, false, includeLeftWall: true);

        // Create second room (now on the left)
        float margin = 1.0f;
        float maxZOffset = (length - secondaryRoomLength) / 2 - margin;
        float randomZOffset = Random.Range(-maxZOffset, maxZOffset);

        Transform room2 = new GameObject("Room2").transform;
        room2.SetParent(parent);
        CreateRoom(room2,
            new Vector3(-width / 2 + secondaryRoomWidth / 2, 0, randomZOffset),
            secondaryRoomWidth, roomHeight, secondaryRoomLength,
            true, includeLeftWall: false); 


        // Adjust shared wall position
        float sharedWallZ = 0;
        if (primaryRoomLength > secondaryRoomLength)
        {
            sharedWallZ = -length / 2 + secondaryRoomLength;
        }
        else
        {
            sharedWallZ = -length / 2 + primaryRoomLength;
        }

        CreateWallWithDoor("SharedWall", 
            new Vector3(internalWallThickness, roomHeight, Mathf.Max(primaryRoomLength, secondaryRoomLength)),
            new Vector3(-width / 2 + secondaryRoomWidth + (internalWallThickness / 2), roomHeight / 2, 0),
            parent, Vector3.right);

    }

    void CreateRoom(Transform roomParent, Vector3 position, float roomWidth, float roomHeight, float roomLength, bool hasExternalDoor, bool includeLeftWall)
    {
        CreateWall("Floor", new Vector3(roomWidth, wallThickness, roomLength), 
                position + new Vector3(0, -wallThickness / 2, 0), roomParent);
        

        if (!includeLeftWall){
        CreateWall("Wall_Left", new Vector3(wallThickness, roomHeight, roomLength), 
                        position + new Vector3(-roomWidth / 2, roomHeight / 2, 0), roomParent);
        }
        
        
        if (includeLeftWall){
        CreateWall("Wall_Right", new Vector3(wallThickness, roomHeight, roomLength), 
                position + new Vector3(roomWidth / 2, roomHeight / 2, 0), roomParent);

        }

        CreateWall("Wall_Back", new Vector3(roomWidth, roomHeight, wallThickness), 
                position + new Vector3(0, roomHeight / 2, roomLength / 2), roomParent);

        if (hasExternalDoor)
        {
            CreateWall("Wall_Front", new Vector3(roomWidth, roomHeight, wallThickness), 
                    position + new Vector3(0, roomHeight / 2, -roomLength / 2), roomParent);
        }
        else
        {
            CreateWallWithDoor("Wall_Front", new Vector3(roomWidth, roomHeight, wallThickness),
                            position + new Vector3(0, roomHeight / 2, -roomLength / 2), roomParent, Vector3.forward);
        }
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

        Debug.Log("OneExternalOneInternal configuration");
    }
}


