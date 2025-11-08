using UnityEngine;
using Pathfinding;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] GameObject[] originRoomList;
    GameObject CreatedRoom;
    [SerializeField] static int curRoomIndex = 0;

    Room currentRoom;
    //private MapGen mapGen;
    public AstarPath astarPath;
    protected override void Awake()
    {
        base.Awake();
        astarPath = GetComponentInChildren<AstarPath>();

    }
    public Room GetCurrentRoom() => currentRoom;
    public void CreateRound()
    {
        SetRoomList(originRoomList);
        CreateRoom();
        ResetAstarPath();
        Vector2 pos = GameManager.Instance.SearchSpawnPoint();
        GameManager.Instance.SetPlayerPos(pos);

    }

    void CreateRoom()
    {
        if (CreatedRoom != null)
            Destroy(CreatedRoom);
        CreatedRoom = Instantiate(originRoomList[curRoomIndex], Vector3.zero, Quaternion.identity);

        CreatedRoom.SetActive(true);
        currentRoom = CreatedRoom.GetComponent<Room>();
        currentRoom.InitRoom();
    }
    public void MoveToRound()
    {
        curRoomIndex++;
        CreateRound();
    }
    void SetRoomList(GameObject[] rooms)
    {
        for (int i = rooms.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (rooms[i], rooms[j]) = (rooms[j], rooms[i]); // swap
        }
    }

    public void ResetAstarPath()
    {
        astarPath.Scan();
    }
}