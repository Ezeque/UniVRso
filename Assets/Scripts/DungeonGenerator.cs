using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] roomPrefabs;
    [SerializeField]
    private GameObject finalRoomPrefab;

    public int roomAmount = 10;
    public Vector2Int gridSize = new Vector2Int(100, 100);

    public GameObject player;
    public int TotalObjectAmount;
    public int currentObjectAmount = 0;

    [SerializeField]
    private Material caveSkybox;

    [SerializeField]
    private Material normalSkybox;

    private Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();
    private Room startRoom;
    private Room finalRoom;
    private Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    void Start()
    {
        if (player == null)
        {
            player = GameObject.Find("Player");
        }
        /* StartCaveCreation(); */
    }

    void GenerateDungeon(GameObject dungeon)
    {
        int roomsLeft = roomAmount - 2;
        Room previousRoom = startRoom;
        Transform parent = dungeon.transform;
        Stack<Room> roomStack = new Stack<Room>(); // Pilha para permitir backtracking

        while (roomsLeft > 0)
        {
            Vector2Int randomDirection = directions[Random.Range(0, directions.Length)];
            Vector2Int potentialLocation = previousRoom.coordinates + randomDirection;

            if (!rooms.ContainsKey(potentialLocation))
            {
                // Criar uma nova sala
                GameObject randomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
                GameObject newRoomObj = Instantiate(randomPrefab, new Vector3(
                    potentialLocation.x * gridSize.x,
                    0,
                    potentialLocation.y * gridSize.y
                ), Quaternion.identity);

                bool shouldCreateObj = Random.Range(0, 10) == 0;
                if ((TotalObjectAmount > currentObjectAmount && shouldCreateObj) || roomsLeft == 0)
                {
                    newRoomObj.GetComponent<Room>().SpawnObject(newRoomObj);
                    currentObjectAmount++;
                }

                Room newRoom = newRoomObj.GetComponent<Room>();
                newRoom.coordinates = potentialLocation;

                ConnectRooms(previousRoom, newRoom);
                rooms.Add(potentialLocation, newRoom);

                previousRoom = newRoom;
                roomsLeft--;
                roomStack.Push(newRoom);
                newRoomObj.transform.SetParent(parent);
            }
            else if (roomStack.Count > 0)
            {
                previousRoom = roomStack.Pop();
            }
        }

        PlaceFinalRoom(previousRoom);
    }


    void PlaceFinalRoom(Room previousRoom)
    {
        Vector2Int randomDirection = directions[Random.Range(0, directions.Length)];
        Vector2Int finalRoomLocation = previousRoom.coordinates + randomDirection;

        while (rooms.ContainsKey(finalRoomLocation))
        {
            randomDirection = directions[Random.Range(0, directions.Length)];
            finalRoomLocation = previousRoom.coordinates + randomDirection;
        }

        GameObject finalRoomObj = Instantiate(finalRoomPrefab, new Vector3(
            finalRoomLocation.x * gridSize.x,
            0,
            finalRoomLocation.y * gridSize.y
        ), Quaternion.identity);

        finalRoom = finalRoomObj.GetComponent<Room>();
        finalRoom.coordinates = finalRoomLocation;

        ConnectRooms(previousRoom, finalRoom);
        rooms.Add(finalRoomLocation, finalRoom);
    }

    void ConnectRooms(Room roomA, Room roomB)
    {
        if (roomA == null || roomB == null)
        {
            Debug.LogError("One of the rooms is null in ConnectRooms.");
            return;
        }

        Vector2Int direction = roomB.coordinates - roomA.coordinates;

        if (direction == Vector2Int.up)
        {
            Destroy(roomA.transform.Find("TopWall")?.gameObject);
            Destroy(roomB.transform.Find("BottomWall")?.gameObject);
        }
        else if (direction == Vector2Int.down)
        {
            Destroy(roomA.transform.Find("BottomWall")?.gameObject);
            Destroy(roomB.transform.Find("TopWall")?.gameObject);
        }
        else if (direction == Vector2Int.left)
        {
            Destroy(roomA.transform.Find("LeftWall")?.gameObject);
            Destroy(roomB.transform.Find("RightWall")?.gameObject);
        }
        else if (direction == Vector2Int.right)
        {
            Destroy(roomA.transform.Find("RightWall")?.gameObject);
            Destroy(roomB.transform.Find("LeftWall")?.gameObject);
        }
    }


    public GameObject StartCaveCreation()
    {
        GameObject dungeon = Instantiate(new GameObject()) as GameObject;
        dungeon.name = "Dungeon";
        Debug.Log(dungeon);
        Transform parent = dungeon.transform;
        Vector2Int startCoordinates = Vector2Int.zero;
        GameObject randomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

        GameObject startRoomObj = Instantiate(randomPrefab, Vector3.zero, Quaternion.identity);
        startRoomObj.transform.Find("SpawnPoint").name = "InitialSpawnPoint";
        startRoom = startRoomObj.GetComponent<Room>();
        startRoom.coordinates = startCoordinates;
        if (rooms.Count > 0) rooms.Clear();
        rooms.Add(startCoordinates, startRoom);
        startRoomObj.transform.SetParent(parent);

        RemoveWallsFromStartRoom(startRoomObj);

        GenerateDungeon(dungeon);
        rooms.Clear();
        return dungeon;
    }

    void RemoveWallsFromStartRoom(GameObject room)
    {
        Destroy(room.transform.Find("TopWall")?.gameObject);
        Destroy(room.transform.Find("BottomWall")?.gameObject);
        Destroy(room.transform.Find("LeftWall")?.gameObject);
        Destroy(room.transform.Find("RightWall")?.gameObject);
    }


    public void EnterCave(Vector3 cavePosition)
    {
        GameObject spawnPoint = GameObject.Find("InitialSpawnPoint");

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = spawnPoint.transform.position;
        player.GetComponent<CharacterController>().enabled = true;
        GameObject environment = GameObject.Find("Environment");
        GameObject.Find("NarrationTrigger").GetComponent<NarrationTriggerController>().environmentController = environment;
        environment.SetActive(false);

        if (caveSkybox != null)
        {
            RenderSettings.skybox = caveSkybox;
            DynamicGI.UpdateEnvironment();
        }

        GameObject.Find("NarrationTrigger").GetComponent<NarrationTriggerController>().normalSkybox = normalSkybox;
    }
}
