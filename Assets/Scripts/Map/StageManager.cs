using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] GameObject[] originRoomList;
    GameObject CreatedRoom;
    [SerializeField] int curRoomIndex = 0;

    Stage currentRoom;
    //private MapGen mapGen;
    public AstarPath astarPath;
    protected override void Awake()
    {
        base.Awake();
        astarPath = GetComponentInChildren<AstarPath>();

    }
    public Stage GetCurrentRoom() => currentRoom;
    public void CreateRound()
    {
        CreateRoom();
        ResetAstarPath();
    }

    void CreateRoom()
    {
        if (CreatedRoom != null)
            Destroy(CreatedRoom);

        CreatedRoom = Instantiate(originRoomList[curRoomIndex], Vector3.zero, Quaternion.identity);
        CreatedRoom.SetActive(true);
        currentRoom = CreatedRoom.GetComponent<Stage>();
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

    public void ResetAstarPath()
    {
        astarPath.Scan();
    }
    public void SelectRoom(int index)
    {
        curRoomIndex = index;
    }
}