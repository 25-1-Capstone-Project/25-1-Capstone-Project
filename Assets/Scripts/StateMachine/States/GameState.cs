using System.Collections;
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

        SceneManager.sceneLoaded += OnSceneMoveMainMenu;

    }

    public override void Update()
    {


    }

    public override void Exit() { }

    void OnSceneMoveMainMenu(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneMoveMainMenu;
        AudioManager.Instance.PlayBGM("Main");
        FadeManager.Instance.FadeIn(Color.black, 1f);
        UIManager.Instance.SetActiveMainMenuUI(true);
        UIManager.Instance.SetActiveStageUI(false);
        UIManager.Instance.SetActiveInGameUI(false);
    }
}


public class StageState : GameState
{
    public StageState(GameManager manager) : base(manager) { }

    public override void Enter()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        gameManager.StartCoroutine(RoomTransitionRoutine("RoundScene"));
        GameManager.Instance.SetTimeScale(1f);

    }
    IEnumerator RoomTransitionRoutine(string nextScene)
    {
        yield return FadeManager.Instance.FadeOut(Color.black, 1f);
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
        FadeManager.Instance.FadeIn(Color.black, 1f);
        AudioManager.Instance.PlayBGM("Room");

        UIManager.Instance.SetActiveMainMenuUI(false);
        UIManager.Instance.SetActiveInGameUI(true);

        StageManager.Instance.CreateRound();
        gameManager.PlayerSpawn();
        CameraManager.Instance.SetActiveCineCam(true);
        CameraManager.Instance.SetCameraPosition(gameManager.playerScript.GetPlayerTransform().position);
        CursorManager.Instance.SetCursorIcon(ECursorType.Aim.GetHashCode(), true);
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

        EnemyManager.Instance.ClearAllEnemies();
        gameManager.StartCoroutine(RoomTransitionRoutine("MainMenu"));
        AudioManager.Instance.StopBGM();
        gameManager.playerScript?.DestroyPlayer();
    }
}



