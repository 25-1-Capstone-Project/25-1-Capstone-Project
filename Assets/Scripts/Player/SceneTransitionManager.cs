using UnityEngine;
using System.Collections;

public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    private EDungeonType dungeonType;
    private EGameState gameState;
   

    protected override void Awake()
    {
        base.Awake();
    }

    public void TransitionScene(EDungeonType dungeonType, EGameState gameState)
    {
        this.dungeonType = dungeonType;

        this.gameState = gameState;

        StartCoroutine(TransitionSceneRoutine());
    }

    private IEnumerator TransitionSceneRoutine()
    {
        yield return FadeController.Instance.FadeOut(Color.black, 1f);

        GameManager.Instance.SetCurrentDungeonType(dungeonType);
        GameManager.Instance.ChangeStateByEnum(gameState);

        // AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetSceneName);
        // while (!loadOp.isDone)
        //     yield return null;

        yield return FadeController.Instance.FadeIn(Color.black, 1f);
    }


}
