
using System.Collections.Generic;
using UnityEngine;
public enum ERoomType
{
    Empty,
    BattleRoom,
    StartRoom,
    BossRoom,
    RewardRoom
}
/// <summary>
/// 아이작의 맵 생성 알고리즘으로 만든 클래스
/// </summary>
/// reference: https://www.boristhebrave.com/2020/09/12/dungeon-generation-in-binding-of-isaac/
///             https://castlejh.tistory.com/40
// Rules:
// Determine the neighbour cell by adding +10/-10/+1/-1 to the currency cell.
//If the neighbour cell is already occupied, give up
// If the neighbour cell itself has more than one filled neighbour, give up.
// If we already have enough rooms, give up
// Random 50% chance, give up
// Otherwise, mark the neighbour cell as having a room in it, and add it to the queue.

public class MapGen : MonoBehaviour
{
    [SerializeField] Vector2 doorDistance;
    public Vector2Int roomgap;
    [SerializeField] int mapWidth;
    [SerializeField] int mapHeight;
    [SerializeField] RoomReference roomData;
    GameObject MapObject;
    [SerializeField] GameObject playerSpawnPoint;
    [SerializeField] int roomCount;
    int[] map;
    int start;
    [SerializeField] GameObject doorPrefab;
    Queue<int> specialRoom;
    bool IsSameRow(int a, int b) => (a / mapWidth) == (b / mapWidth);
    int[] depthMap;
    public void SetRoomData(RoomReference roomData)
    {
        this.roomData = roomData;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GenerateMap();
        }
    }
    //맵 초기화
    public void GenerateMap()
    {
        if (MapObject != null) Destroy(MapObject);
        MapObject = new GameObject("GeneratedMap");

        MapCreate();
        SetSpecialRoom();
        SpawnRoom();
    }
    public void CreatePlayerSpawnPoint(Vector2 pos)
    {
        Instantiate(playerSpawnPoint, pos, Quaternion.identity);
    }
    private bool IsNotEmptyRoom(int value)
    {
        if (value == ERoomType.BattleRoom.GetHashCode())
        {
            return true;
        }
        else if (value == ERoomType.StartRoom.GetHashCode())
        {
            return true;
        }
        return false;
    }
    //맵 생성
    public void MapCreate()
    {
        bool success = false;

        while (!success)
        {
            map = new int[mapWidth * mapHeight];
            int[] offsets = { mapWidth, -mapWidth, 1, -1 };
            depthMap = new int[map.Length];
            Queue<int> roomQueue = new Queue<int>();
            specialRoom = new Queue<int>();
            start = mapWidth * mapHeight / 2 + mapWidth / 2;
            depthMap[start] = 0;
            roomQueue.Enqueue(start);
            map[start] = ERoomType.StartRoom.GetHashCode();
            int _roomCount = roomCount - 1;

            while (roomQueue.Count > 0)
            {
                int index = roomQueue.Dequeue();
                int currentDepth = depthMap[index];
                bool isRoomCreated = false;
                foreach (int offset in offsets)
                {
                    int newIndex = index + offset;
                    if (newIndex < 0 || newIndex >= map.Length) continue;
                    if ((offset == 1 || offset == -1) && !IsSameRow(index, newIndex)) continue;
                    if (_roomCount == 0) continue;
                    if (IsNotEmptyRoom(map[newIndex])) continue;

                    int count = 0;
                    foreach (int offset2 in offsets)
                    {
                        int neighbor = newIndex + offset2;
                        if (neighbor < 0 || neighbor >= map.Length) continue;
                        if (IsNotEmptyRoom(map[neighbor])) count++;
                    }

                    if (count != 1) continue;
                    if (Random.value > 0.5f) continue;

                    map[newIndex] = 1;
                    depthMap[newIndex] = currentDepth + 1;
                    roomQueue.Enqueue(newIndex);
                    isRoomCreated = true;
                    _roomCount--;
                }
                if (!isRoomCreated) specialRoom.Enqueue(index);
            }

            if (_roomCount <= 0)
            {
                success = true;
            }
        }
    }
    //방 생성
    public void SpawnRoom()
    {
        MinimapManager.Instance.InitMiniMap();
        MapManager.Instance.roomMap.Clear();
        GameObject roomPrefab = null;
        Vector2Int roomPos = new Vector2Int();

        for (int i = 0; i < map.Length; i++)
        {
            if (map[i] == ERoomType.Empty.GetHashCode())
            {
                continue;
            }
            else if (map[i] == ERoomType.BattleRoom.GetHashCode())
            {
                roomPrefab = roomData.GetRandomRoom(depthMap[i]);
                roomPos = new Vector2Int(i % mapWidth, i / mapWidth);
            }
            else if (map[i] == ERoomType.StartRoom.GetHashCode())
            {
                roomPrefab = roomData.GetStartRoom();
                roomPos = new Vector2Int(i % mapWidth, i / mapWidth);
            }
            else if (map[i] == ERoomType.BossRoom.GetHashCode())
            {
                roomPrefab = roomData.BossRooms[Random.Range(0, roomData.BossRooms.Length)];
                roomPos = new Vector2Int(i % mapWidth, i / mapWidth);
            }
            else if (map[i] == ERoomType.RewardRoom.GetHashCode())
            {
                roomPrefab = roomData.RewardRooms[Random.Range(0, roomData.RewardRooms.Length)];
                roomPos = new Vector2Int(i % mapWidth, i / mapWidth);
            }
            CreateRoom(roomPrefab, roomPos);

        }

        Vector2Int startRoomPos = new Vector2Int(start % mapWidth, start / mapWidth);
        MapManager.Instance.currentRoomPos = startRoomPos;
        MapManager.Instance.roomMap[startRoomPos].SetActive(true);
        MapManager.Instance.roomMap[startRoomPos].GetComponent<Room>().ClearRoom();
        CreatePlayerSpawnPoint(MapManager.Instance.roomMap[startRoomPos].transform.position);

    }

    private void CreateRoom(GameObject roomPrefab, Vector2Int roomPos)
    {
        GameObject room = Instantiate(roomPrefab, new Vector3(roomPos.x * (roomgap.x + roomgap.x), roomPos.y * (doorDistance.y + roomgap.y), 0), Quaternion.identity, MapObject.transform);

        MapManager.Instance.roomMap.Add(roomPos, room);


        AddDoorIfNeighborExists(room, roomPos + Vector2Int.up, Direction.Up, new Vector3(0, doorDistance.y / 2, 0));
        AddDoorIfNeighborExists(room, roomPos + Vector2Int.down, Direction.Down, new Vector3(0, -doorDistance.y / 2, 0));
        AddDoorIfNeighborExists(room, roomPos + Vector2Int.left, Direction.Left, new Vector3(-doorDistance.x / 2, 0, 0));
        AddDoorIfNeighborExists(room, roomPos + Vector2Int.right, Direction.Right, new Vector3(doorDistance.x / 2, 0, 0));
        room.GetComponent<Room>().InitRoom();

        // minimap 등록
        MinimapManager.Instance.RegisterRoom(roomPos);
    }

    void AddDoorIfNeighborExists(GameObject roomObj, Vector2Int neighborPos, Direction dir, Vector3 localPos)
    {
        if (MapManager.Instance.roomMap.ContainsKey(neighborPos) ||
            (neighborPos.x >= 0 && neighborPos.x < mapWidth &&
             neighborPos.y >= 0 && neighborPos.y < mapHeight &&
             (map[neighborPos.x + neighborPos.y * mapWidth] == 1 || map[neighborPos.x + neighborPos.y * mapWidth] == 2)))
        {

            GameObject door = Instantiate(doorPrefab, roomObj.transform);
            door.transform.localPosition = localPos;
            door.name = $"Door_{dir}";

            DoorTrigger trigger = door.GetComponent<DoorTrigger>();
            trigger.direction = dir;

            roomObj.GetComponent<Room>().AddPortalPointObj(door);
        }
    }
    [SerializeField] int rewardRoomCount = 2;
    [SerializeField] int bossRoomCount = 1;
    int totalRoomCount => rewardRoomCount + bossRoomCount;

    //가장 바깥쪽 방에 보상방과 보스방 설정
    private void SetSpecialRoom()
    {
        int[] temp = new int[specialRoom.Count];

        //가장먼 최대 값 찾아서 정렬
        for (int i = 0; specialRoom.Count > 0;)
            temp[i++] = specialRoom.Dequeue();

        // depthMap을 기준으로 정렬 (깊이 기준으로 내림차순)
        System.Array.Sort(temp, (a, b) => depthMap[b].CompareTo(depthMap[a]));

        map[temp[0]] = ERoomType.BossRoom.GetHashCode(); // 가장 먼 방 → 보스룸

        for (int i = 1; i <= rewardRoomCount; i++)
        {
            map[temp[i]] = ERoomType.RewardRoom.GetHashCode(); // 그 다음들 → 보상룸
        }
    }

}
