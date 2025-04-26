using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
public class GameManager : Singleton<GameManager>
{
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
 

}

