using Unity.VisualScripting;
using UnityEngine;


public enum EDungeonType { Null = -1, BlueDragon, WhiteTiger, RedBird, BlackTortoise }
public enum EGameState { MainMenu, Hub, Dungeon, Paused };
public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject PlayerPrefab;
    public StateMachine<GameState> StateMachine { get; private set; }
    private GameObject playerObj;
    public EDungeonType currentDungeonType = EDungeonType.Null;
    public MapReference[] mapData;
    public int CurrentDungeonFloor { get; set; } = 0;
    public int MaxDungeonFloor { get; private set; } = 3;

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1f;
    }

    void Start()
    {
        StateMachine = new StateMachine<GameState>();
        StateMachine.AddState(new MainMenuState(this));
        StateMachine.AddState(new HubState(this));
        StateMachine.AddState(new DungeonState(this));
        StateMachine.AddState(new PausedState(this));

        StateMachine.ChangeState<MainMenuState>();
    }

    void Update()
    {
        StateMachine.Update();
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void SetCurrentDungeonType(EDungeonType eDungeonType)
    {
        currentDungeonType = eDungeonType;
    }
    public void InstancePlayer()
    {
        if (playerObj == null)
            playerObj = Instantiate(PlayerPrefab);
    }

    public Vector2 SearchSpawnPoint()
    {
        return GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform.position;
    }


    public void ChangeStateByEnum(EGameState gameState)
    {
        switch (gameState)
        {
            case EGameState.MainMenu:
                StateMachine.ChangeState<MainMenuState>();
                break;
            case EGameState.Hub:
                StateMachine.ChangeState<HubState>();
                break;
            case EGameState.Dungeon:
                StateMachine.ChangeState<DungeonState>();
                break;
            case EGameState.Paused:
                StateMachine.ChangeState<PausedState>();
                break;
            default:
                Debug.LogWarning($"Unknown GameState: {gameState}");
                break;
        }
    }

    public void GoToNextDungeonFloor()
    {
        CurrentDungeonFloor++;
        if (CurrentDungeonFloor >= MaxDungeonFloor)
        {
            Debug.Log("Dungeon cleared! Returning to Hub.");
            ReturnToHub();
        }
        else
        {
            StateMachine.ChangeState<DungeonState>();
        }
    }

    public void ReturnToHub()
    {
        CurrentDungeonFloor = 0;
        StateMachine.ChangeState<HubState>();
    }

    public void PlayerSpawn()
    {
        Vector2 targetPos = SearchSpawnPoint();
        PlayerScript.Instance.SetPlayerPosition(targetPos);
        CameraManager.Instance.SetCameraPosition(targetPos);
    }
    public void SetPlayerPos(Vector2 pos)
    {
        PlayerScript.Instance.SetPlayerPosition(pos);
        CameraManager.Instance.SetCameraPosition(pos);
    }

}