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
    [SerializeField] int mapWidth = 10;
    [SerializeField] int mapHeight = 10;
    [SerializeField] GameObject roomPrefab;
    GameObject MapObject;
    [SerializeField] int roomCount;
    int _roomCount;
    int[] map;

    List<int> SpecialRoom;
    public void Start()
    {
        MapCreate();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (MapObject != null) Destroy(MapObject);
            MapObject = new GameObject();

            MapCreate();
            SpawnRoom();
        }
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
        _roomCount = roomCount - 1;
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
        for (int i = 0; i < map.Length; i++)
        {
            if (map[i] == 1)
            {
                if (SpecialRoom.Contains(i))
                {
                    GameObject room = Instantiate(roomPrefab, new Vector2(i % mapWidth, i / mapWidth), Quaternion.identity, MapObject.transform);
                    room.GetComponent<SpriteRenderer>().color = Color.yellow;
                }
                else
                {
                    GameObject room = Instantiate(roomPrefab, new Vector2(i % mapWidth, i / mapWidth), Quaternion.identity, MapObject.transform);
                    room.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
    }
    bool IsSameRow(int a, int b) => (a / mapWidth) == (b / mapWidth);
}
