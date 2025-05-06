using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject PlayerPrefab;
    public StateMachine<GameState> StateMachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1f;
    }

    void Start()
    {
        StateMachine = new StateMachine<GameState>();
        StateMachine.AddState(new MainMenuState(this));
        StateMachine.AddState(new PlayingState(this));
        StateMachine.AddState(new PausedState(this));

        StateMachine.ChangeState<MainMenuState>();
    }

    void Update()
    {
        StateMachine.Update();
    }

    public void TestResetButton()
    {
        SceneManager.LoadScene(0);
        PlayerScript.Instance.InitPlayer();
    }
    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
    public void InstancePlayer(Vector2 spawnPoint)
    {
        Instantiate(PlayerPrefab, spawnPoint, Quaternion.identity);
        CameraManager.Instance.SetCameraPosition(spawnPoint);

    }

    public void ChangeStateByName(string stateName)
    {
        if (stateName == "MainMenu")
        {
            StateMachine.ChangeState<MainMenuState>();
        }
        else if (stateName == "Playing")
        {
            StateMachine.ChangeState<PlayingState>();
        }
        else if (stateName == "Paused")
        {
            StateMachine.ChangeState<PausedState>();
        }

    }

    public Vector2 SearchSpawnPoint()
    {
        Vector2 spawnT = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform.position;
        return spawnT;
    }
}

