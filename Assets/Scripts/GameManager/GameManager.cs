using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public enum EDungeonType { Null = -1, BlueDragon, WhiteTiger, RedBird, BlackTortoise }
public enum EGameState { MainMenu, Room, Paused };
public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject PlayerPrefab;
    public StateMachine<GameState> StateMachine { get; private set; }
    private GameObject playerObj;
    public PlayerScript playerScript;
    public EDungeonType currentDungeonType = EDungeonType.Null;
    // public MapReference[] mapData;
    public int CurrentDungeonFloor { get; set; } = 0;
    public int MaxDungeonFloor { get; private set; } = 3;
    Coroutine timeScaleCoroutine;
    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1f;
    }

    void Start()
    {
        StateMachine = new StateMachine<GameState>();
        StateMachine.AddState(new MainMenuState(this));
        StateMachine.AddState(new StageState(this));
        StateMachine.ChangeState<MainMenuState>();
    }

    void Update()
    {
        StateMachine.Update();
    }

    public void SetTimeScale(float targetTimeScale, float duration = 0)
    {
        if (duration <= 0)
        {
            Time.timeScale = targetTimeScale;
            return;
        }
        if (timeScaleCoroutine != null)
            StopCoroutine(timeScaleCoroutine);
        timeScaleCoroutine = StartCoroutine(LerpTimeScale(targetTimeScale, duration));
    }

    private IEnumerator LerpTimeScale(float targetTimeScale, float duration)
    {
        float startTimeScale = Time.timeScale;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeScale, elapsedTime / duration);
            yield return null;
        }

        Time.timeScale = targetTimeScale;
    }


    public void SetCurrentDungeonType(EDungeonType eDungeonType)
    {
        currentDungeonType = eDungeonType;
    }
    public void InstancePlayer()
    {
        if (playerObj != null) return;

        playerObj = Instantiate(PlayerPrefab);
        playerScript = playerObj.GetComponent<PlayerScript>();
    }

    public Vector2 SearchSpawnPoint()
    {
        return GameObject.FindWithTag("PlayerSpawnPoint").transform.position;
    }


    public void ChangeStateByEnum(EGameState gameState)
    {
        switch (gameState)
        {
            case EGameState.MainMenu:
                StateMachine.ChangeState<MainMenuState>();
                break;
            case EGameState.Room:
                StateMachine.ChangeState<StageState>();
                break;
            default:
                Debug.LogWarning($"Unknown GameState: {gameState}");
                break;
        }
    }

    // public void GoToNextDungeonFloor()
    // {
    //     CurrentDungeonFloor++;
    //     if (CurrentDungeonFloor >= MaxDungeonFloor)
    //     {
    //         Debug.Log("Dungeon cleared! Returning to Hub.");
    //         StartButton();
    //     }
    //     else
    //     {
    //         StateMachine.ChangeState<DungeonState>();
    //     }
    // }

    public void StartButton()
    {
        //  StateMachine.ChangeState<DungeonState>();
        UIManager.Instance.SetActiveMainMenuUI(false);
        UIManager.Instance.SetActiveStageUI(true);
    }

    public void PlayerSpawn()
    {
        Vector2 targetPos = SearchSpawnPoint();
        playerScript.SetPlayerPosition(targetPos);
        CameraManager.Instance.SetCameraPosition(targetPos);
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


}