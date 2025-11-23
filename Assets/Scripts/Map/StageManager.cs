using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] GameObject[] originRoomList;
    GameObject CreatedRoom;
    [SerializeField] int curRoomIndex = 0;

    Stage currentStage;
    //private MapGen mapGen;
    public AstarPath astarPath;
    protected override void Awake()
    {
        base.Awake();
        astarPath = GetComponentInChildren<AstarPath>();

    }
    public Stage GetCurrentStage() => currentStage;
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
        currentStage = CreatedRoom.GetComponent<Stage>();
        currentStage.InitRoom();
    }
    public void NextRound()
    {
        curRoomIndex++;
        CreateRound();

        // 플레이어를 초기화
        GameManager.Instance.playerScript.gameObject.SetActive(false);
        GameManager.Instance.playerScript.InitPlayer();

        // 새 스폰 포인트에서 스폰
        GameManager.Instance.PlayerSpawn();
        GameManager.Instance.playerScript.gameObject.SetActive(true);

        AudioManager.Instance.PlayBGM("Room");
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