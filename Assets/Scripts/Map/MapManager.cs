using UnityEngine;
using System.Collections.Generic;

public class MapManager : Singleton<MapManager>
{
    public Dictionary<Vector2Int, GameObject> roomMap = new Dictionary<Vector2Int, GameObject>();
    public Vector2Int currentRoomPos;
    private MapGen mapGen;
   
    protected override void Awake()
    {
        base.Awake();
        mapGen = GetComponent<MapGen>();
       
    }
    public Room GetCurrentRoom() => roomMap[currentRoomPos].GetComponent<Room>();
    public void CreateMap()
    {
        mapGen.GenerateMap();
    }
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

        // 비활성화
        roomMap[currentRoomPos].SetActive(false);

        // 활성화
        roomMap[nextPos].SetActive(true);
        
        // 플레이어 위치 이동 (새 방의 반대편 문 위치로)
        MinimapManager.Instance.RevealRoom(nextPos);
        Vector3 entryPoint = FindEntryPoint(nextPos, dir);
        PlayerScript.Instance.SetPlayerPosition(entryPoint);
        CameraManager.Instance.SetCameraPosition(roomMap[nextPos].transform.position);
        currentRoomPos = nextPos;

    }

    private Vector3 FindEntryPoint(Vector2Int roomPos, Direction fromDirection)
    {
        GameObject room = roomMap[roomPos];
      //  PlayerScript.Instance.SetGroundTilemap(room.GetComponent<Room>().GroundTileMap);
        string entryDoorName = fromDirection
        switch
        {
            Direction.Up => "Door_Down",
            Direction.Down => "Door_Up",
            Direction.Left => "Door_Right",
            Direction.Right => "Door_Left",
            _ => "Door_Down"
        };
        return room.transform.Find(entryDoorName).position + fromDirection switch
        {
            Direction.Up => new Vector3(0, 1, 0),
            Direction.Down => new Vector3(0, -1, 0),
            Direction.Left => new Vector3(-1, 0, 0),
            Direction.Right => new Vector3(1, 0, 0),
            _ => Vector3.zero
        };
    }
    public void SetActiveMapManager(bool active){
            gameObject.SetActive(active);
    }
}