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

public class HubState : GameState
{
    public HubState(GameManager manager) : base(manager) { }

    public override void Enter()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("HubScene");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        SceneManager.sceneLoaded -= OnSceneLoaded;
        gameManager.InstancePlayer();
        gameManager.PlayerSpawn();
        PlayerScript.Instance.InitPlayer();
        UIManager.Instance.SetActiveMainMenuUI(false);
        UIManager.Instance.SetActiveInGameUI(true);
       // CameraManager.Instance.SetActiveCineCam(true);
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

        string sceneName =
        gameManager.mapData[(int)gameManager.currentDungeonType]
        .sceneNames[gameManager.CurrentDungeonFloor];

        SceneManager.LoadScene(sceneName);
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        MapManager.Instance.CreateMap();
        UIManager.Instance.SetActiveMainMenuUI(false);
        UIManager.Instance.SetActiveInGameUI(true);
        CameraManager.Instance.SetActiveCineCam(false);
        EnemyManager.Instance.InitSpawnedEnemy();

        gameManager.PlayerSpawn();
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

    public override void Exit() { }
}



