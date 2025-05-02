using UnityEngine;
using System.Collections.Generic;

public class RoomManager : Singleton<RoomManager>
{
    public Dictionary<Vector2Int, GameObject> roomMap = new Dictionary<Vector2Int, GameObject>();
    public Vector2Int currentRoomPos;

    public GameObject player;


    public void MoveToRoom(Direction dir)
    {
        Vector2Int nextPos = currentRoomPos;
        switch (dir)
        {
            case Direction.Up: nextPos += Vector2Int.up; break;
            case Direction.Down: nextPos += Vector2Int.down; break;
            case Direction.Left: nextPos += Vector2Int.left; break;
            case Direction.Right: nextPos += Vector2Int.right; break;
        }

        if (roomMap.ContainsKey(nextPos))
        {
            // 비활성화
          //  roomMap[currentRoomPos].SetActive(false);

            // 활성화
            roomMap[nextPos].SetActive(true);

            // 플레이어 위치 이동 (새 방의 반대편 문 위치로)
            Transform entryPoint = FindEntryPoint(nextPos, dir);
            player.transform.position = entryPoint.position;

            currentRoomPos = nextPos;
        }
    }

    private Transform FindEntryPoint(Vector2Int roomPos, Direction fromDirection)
    {
        GameObject room = roomMap[roomPos];
        string entryDoorName = fromDirection switch
        {
            Direction.Up => "Door_Down",
            Direction.Down => "Door_Up",
            Direction.Left => "Door_Right",
            Direction.Right => "Door_Left",
            _ => "Door_Down"
        };
        return room.transform.Find(entryDoorName);
    }
}