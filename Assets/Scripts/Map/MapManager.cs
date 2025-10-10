using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] GameObject[] originRoomList;
    GameObject[] CreatedRoomList;
    Room currentRoom;
    [SerializeField] int curRoomIndex = 0;

    //private MapGen mapGen;
    public AstarPath astarPath;
    protected override void Awake()
    {
        base.Awake();
       // mapGen = GetComponent<MapGen>();
        astarPath = GetComponentInChildren<AstarPath>();

    }
    public Room GetCurrentRoom() => CreatedRoomList[curRoomIndex].GetComponent<Room>();
    public void CreateMap()
    {
        CreatedRoomList = new GameObject[originRoomList.Length];
        SetRoomList(originRoomList);
        CreateRoom();
        currentRoom.SpawnEnemies();
        ResetAstarPath();
        // mapGen.GenerateMap();
    }

    void CreateRoom()
    {
        
        for (int i = 0; i < originRoomList.Length; i++)
        {
            CreatedRoomList[i] = Instantiate(originRoomList[i], Vector2.right * i * 30, Quaternion.identity);
            CreatedRoomList[i].SetActive(false);
        }
        CreatedRoomList[0].SetActive(true);
        currentRoom = CreatedRoomList[0].GetComponent<Room>();
    }
    void SetRoomList(GameObject[] rooms)
    {
        for (int i = rooms.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); 
            (rooms[i], rooms[j]) = (rooms[j], rooms[i]); // swap
        }
    }
    public void MoveToRoom()
    {
        //방 이동
        CreatedRoomList[curRoomIndex].SetActive(false);
        curRoomIndex++;
        CreatedRoomList[curRoomIndex].SetActive(true);

        //플레이어 위치 이동
        Vector2 pos = GameManager.Instance.SearchSpawnPoint();
        GameManager.Instance.SetPlayerPos(pos);

        currentRoom = CreatedRoomList[curRoomIndex].GetComponent<Room>();
        currentRoom.SpawnEnemies();
        ResetAstarPath();
    }
    public void ResetAstarPath()
    {
        AstarData.active.data.gridGraph.center = CreatedRoomList[curRoomIndex].transform.position;
        astarPath.Scan();

    }
}