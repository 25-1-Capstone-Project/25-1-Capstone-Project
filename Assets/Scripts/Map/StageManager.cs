using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] GameObject[] originRoomList;
    GameObject CreatedRoom;
    [SerializeField] int curRoomIndex = 0;

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
        // SetRoomList(originRoomList);
        CreateRoom();
        ResetAstarPath();
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
    public void NextRound()
    {
        curRoomIndex++;
        CreateRound();
        AudioManager.Instance.PlayBGM("Room");
        GameManager.Instance.playerScript.InitPlayer();
        GameManager.Instance.PlayerSpawn();
        UIManager.Instance.successUI.SetActiveDeadInfoPanel(false);
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
    public void SelectRoom(int index)
    {
        curRoomIndex = index;
    }
}