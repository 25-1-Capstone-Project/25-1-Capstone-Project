using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //싱글톤
    public static GameManager instance;
    public Player player;

    public StateMachine<GameState> StateMachine { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
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

    public void TestResetButton(){
        SceneManager.LoadScene(0);
    }
      // Public methods to expose data or perform actions
    public Transform GetPlayerTransform()
    {
        return player.transform;
    }
}

