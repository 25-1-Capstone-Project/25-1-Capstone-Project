using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public abstract class GameState : IState
{
    protected GameManager gameManager;
    public GameState(GameManager manager)
    {
        gameManager = manager;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

public class MainMenuState : GameState
{
    public MainMenuState(GameManager manager) : base(manager) { }

    public override void Enter()
    {
        CursorManager.Instance.SetCursorIcon(ECursorType.Default.GetHashCode());



    }

    public override void Update()
    {


    }

    public override void Exit() { }
}

// public class StageState : GameState
// {
//     public StageState(GameManager manager) : base(manager) { }

//     public override void Enter()
//     {
//         SceneManager.sceneLoaded += OnSceneLoaded;
//         SceneManager.LoadScene("HubScene");
//     }

//     void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {
//         SceneManager.sceneLoaded -= OnSceneLoaded;
//         gameManager.InstancePlayer();
//         gameManager.PlayerSpawn(gameManager.SearchSpawnPoint());
//         PlayerScript.Instance.InitPlayer();
//         UIManager.Instance.SetActiveMainMenuUI(false);
//         UIManager.Instance.SetActiveInGameUI(true);

//     }

//     public override void Update()
//     {
//         // if (Input.GetKeyDown(KeyCode.Space))
//         // {
//         //     gameManager.CurrentDungeonFloor = 0;
//         //     gameManager.StateMachine.ChangeState<DungeonState>();
//         // }
//         if (Input.GetKeyDown(KeyCode.Escape))
//         {
//             UIManager.Instance.pauseUI.TogglePause();
//         }
//     }

//     public override void Exit() { }
// }

public class RoomState : GameState
{
    public RoomState(GameManager manager) : base(manager) { }

    public override void Enter()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        gameManager.StartCoroutine(RoomTransitionRoutine("RoundScene"));
        GameManager.Instance.SetTimeScale(1f);

    }
    IEnumerator RoomTransitionRoutine(string nextScene)
    {
        yield return FadeController.Instance.FadeOut(Color.black, 1f);
        SceneManager.LoadScene(nextScene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        gameManager.InstancePlayer();
        gameManager.playerScript.InitPlayer();
        UIManager.Instance.SetActiveMainMenuUI(false);
        UIManager.Instance.SetActiveStageUI(false);
        UIManager.Instance.SetActiveInGameUI(true);
        FadeController.Instance.FadeIn(Color.black, 1f);
        AudioManager.Instance.PlayBGM("Room");

        UIManager.Instance.SetActiveMainMenuUI(false);
        UIManager.Instance.SetActiveInGameUI(true);

        StageManager.Instance.CreateRound();
        gameManager.PlayerSpawn(gameManager.SearchSpawnPoint());
        CameraManager.Instance.SetActiveCineCam(true);
        CameraManager.Instance.SetCameraPosition(gameManager.SearchSpawnPoint());
        CursorManager.Instance.SetCursorIcon(ECursorType.Aim.GetHashCode(), true);
    }
    void OnSceneMoveMainMenu(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneMoveMainMenu;
        FadeController.Instance.FadeIn(Color.black, 1f);
        UIManager.Instance.SetActiveMainMenuUI(true);
        UIManager.Instance.SetActiveStageUI(false);
        UIManager.Instance.SetActiveInGameUI(false);
    }
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.pauseUI.TogglePause();
        }
    }

    public override void Exit()
    {
         SceneManager.sceneLoaded += OnSceneMoveMainMenu;
        gameManager.StartCoroutine(RoomTransitionRoutine("MainMenu"));
        AudioManager.Instance.StopBGM();
        gameManager.playerScript?.DestroyPlayer();
    }
}



