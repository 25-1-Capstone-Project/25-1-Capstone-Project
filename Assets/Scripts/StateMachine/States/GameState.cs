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
        SceneManager.LoadScene("MainMenu");
        UIManager.Instance.SetActiveMainMenuUI(true);
    }

    public override void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        //     gameManager.StateMachine.ChangeState<HubState>();
    }

    public override void Exit() { }
}

public class StageState : GameState
{
    public StageState(GameManager manager) : base(manager) { }

    public override void Enter()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("HubScene");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        gameManager.InstancePlayer();
        gameManager.PlayerSpawn(gameManager.SearchSpawnPoint());
        PlayerScript.Instance.InitPlayer();
        UIManager.Instance.SetActiveMainMenuUI(false);
        UIManager.Instance.SetActiveInGameUI(true);

    }

    public override void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     gameManager.CurrentDungeonFloor = 0;
        //     gameManager.StateMachine.ChangeState<DungeonState>();
        // }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.pauseUI.TogglePause();
        }
    }

    public override void Exit() { }
}

public class DungeonState : GameState
{
    public DungeonState(GameManager manager) : base(manager) { }

    public override void Enter()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("RoundScene");
        // string sceneName =
        // gameManager.mapData[(int)gameManager.currentDungeonType]
        // .sceneNames[gameManager.CurrentDungeonFloor];
        AudioManager.Instance.PlayBGM("Room");

    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        UIManager.Instance.SetActiveMainMenuUI(false);
        UIManager.Instance.SetActiveInGameUI(true);
        gameManager.InstancePlayer();
        PlayerScript.Instance.InitPlayer();
        StageManager.Instance.CreateRound();
        gameManager.PlayerSpawn(gameManager.SearchSpawnPoint());
        CameraManager.Instance.SetActiveCineCam(true);
        CameraManager.Instance.SetCameraPosition(gameManager.SearchSpawnPoint());
        CursorManager.Instance.SetCursorIcon(ECursorType.Aim.GetHashCode(), true);
    }

    public override void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     gameManager.GoToNextDungeonFloor();
        // }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.pauseUI.TogglePause();
        }
    }

    public override void Exit()
    {
        AudioManager.Instance.StopBGM();
    }
}



