using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
// 게임 상태 기본 클래스
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

// MainMenu 상태
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
        if (Input.GetKeyDown(KeyCode.Space))
            gameManager.StateMachine.ChangeState<PlayingState>();
    }

    public override void Exit()
    {

    }
}

// 게임 진행 중 상태
public class PlayingState : GameState
{
    public PlayingState(GameManager manager) : base(manager) { }

    public override void Enter()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("CenterForestScene");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트 제거

        UIManager.Instance.SetActiveMainMenuUI(false);
        UIManager.Instance.SetActiveInGameUI(true);
        gameManager.InstancePlayer(GameManager.Instance.SearchSpawnPoint());
        CameraManager.Instance.SetActiveCineCam(true);
    }
    public override void Update()
    {

    }

    public override void Exit()
    {
        Debug.Log("게임 중단");
    }
}

// 일시정지 상태
public class PausedState : GameState
{
    public PausedState(GameManager manager) : base(manager) { }

    public override void Enter()
    {
        Debug.Log("게임 일시 정지");
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            gameManager.StateMachine.ChangeState<PlayingState>();
    }

    public override void Exit()
    {
        Debug.Log("게임 다시 시작");
    }
}
