using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] Vector2Int roomSize;
    [SerializeField] int mapWidth = 10;
    [SerializeField] int mapHeight = 10;
    [SerializeField] GameObject roomPrefab;
    GameObject MapObject;
    [SerializeField] int roomCount;
    int[] map;
    [SerializeField] GameObject doorPrefab;
    List<int> SpecialRoom;

    public void InitMap()
    {
        if (MapObject != null) Destroy(MapObject);
        MapObject = new GameObject("GeneratedMap");

        MapCreate();
        SpawnRoom();
    }

    public void MapCreate()
    {
        map = new int[mapWidth * mapHeight];
        int[] offsets = { mapWidth, -mapWidth, 1, -1 };
        Queue<int> roomQueue = new Queue<int>();
        SpecialRoom = new List<int>();
        int start = mapWidth * mapHeight / 2 + mapWidth / 2;
        roomQueue.Enqueue(start);
        map[start] = 1;
        int _roomCount = roomCount - 1;
        while (roomQueue.Count > 0)
        {
            int index = roomQueue.Dequeue();
            bool isRoomCreated = false;
            foreach (int offset in offsets)
            {
                int newIndex = index + offset;
                if (newIndex < 0 || newIndex >= map.Length) continue;

                if ((offset == 1 || offset == -1) && !IsSameRow(index, newIndex)) continue;
                if (_roomCount == 0) continue;
                if (map[newIndex] == 1) continue;

                int count = 0;
                foreach (int offset2 in offsets)
                {
                    int neighbor = newIndex + offset2;
                    if (neighbor < 0 || neighbor >= map.Length) continue;
                    if (map[neighbor] == 1) count++;
                }


                if (count != 1) continue;

                if (Random.value > 0.5f) continue;

                map[newIndex] = 1;
                roomQueue.Enqueue(newIndex);
                isRoomCreated = true;
                _roomCount--;
            }
            if (!isRoomCreated) SpecialRoom.Add(index);
        }

        if (_roomCount > 0)
        {
            MapCreate();
        }

    }
    public void SpawnRoom()
    {
        MapManager.Instance.roomMap.Clear();

        for (int i = 0; i < map.Length; i++)
        {
            if (map[i] == 1)
            {
                Vector2Int roomPos = new Vector2Int(i % mapWidth, i / mapWidth);
                GameObject room = Instantiate(roomPrefab, new Vector3(roomPos.x * roomSize.x, roomPos.y * roomSize.y, 0), Quaternion.identity, MapObject.transform);

                MapManager.Instance.roomMap.Add(roomPos, room);
                room.SetActive(false);

                AddDoorIfNeighborExists(room, roomPos + Vector2Int.up, Direction.Up, new Vector3(0, roomSize.y / 2, 0));
                AddDoorIfNeighborExists(room, roomPos + Vector2Int.down, Direction.Down, new Vector3(0, -roomSize.y / 2, 0));
                AddDoorIfNeighborExists(room, roomPos + Vector2Int.left, Direction.Left, new Vector3(-roomSize.x / 2, 0, 0));
                AddDoorIfNeighborExists(room, roomPos + Vector2Int.right, Direction.Right, new Vector3(roomSize.x / 2, 0, 0));
                room.GetComponent<Room>().InitRoom();
            }
        }

        Vector2Int startRoomPos = new Vector2Int(mapWidth / 2, mapHeight / 2);
        MapManager.Instance.currentRoomPos = startRoomPos;
        MapManager.Instance.roomMap[startRoomPos].SetActive(true);

        GameManager.Instance.InstancePlayer(MapManager.Instance.roomMap[startRoomPos].transform.position);
    }

    void AddDoorIfNeighborExists(GameObject roomObj, Vector2Int neighborPos, Direction dir, Vector3 localPos)
    {
        if (MapManager.Instance.roomMap.ContainsKey(neighborPos) ||
            (neighborPos.x >= 0 && neighborPos.x < mapWidth &&
             neighborPos.y >= 0 && neighborPos.y < mapHeight &&
             map[neighborPos.x + neighborPos.y * mapWidth] == 1))
        {

            GameObject door = Instantiate(doorPrefab, roomObj.transform);
            door.transform.localPosition = localPos;
            door.name = $"Door_{dir}";

            DoorTrigger trigger = door.GetComponent<DoorTrigger>();
            trigger.direction = dir;

            roomObj.GetComponent<Room>().AddPortalPointObj(door);

            
        }
    }
    bool IsSameRow(int a, int b) => (a / mapWidth) == (b / mapWidth);


}
