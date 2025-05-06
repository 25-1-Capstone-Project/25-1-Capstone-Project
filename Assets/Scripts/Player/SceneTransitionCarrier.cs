using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneTransitionCarrier : Singleton<SceneTransitionCarrier>
{

    public string targetSceneName;
    public int spawnPointID;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void TransitionScene(string targetSceneName, int targetSpawnPointID)
    {
        this.targetSceneName = targetSceneName;
        this.spawnPointID = targetSpawnPointID;
        StartCoroutine(TransitionSceneRoutine());
    }
    private IEnumerator TransitionSceneRoutine()
    {

        // 페이드 아웃
        yield return FadeController.Instance.FadeOut(Color.black, 1f);

        // 씬 로딩
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetSceneName);

        while (!loadOp.isDone)
            yield return null;

    }
    void PlayerSpawn()
    {
        if (SceneTransitionCarrier.Instance == null) return;

        int spawnID = spawnPointID;

        var spawns = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
        foreach (var spawn in spawns)
        {
            if (spawn.spawnPointID == spawnID)
            {
                PlayerScript.Instance.SetPlayerPosition(spawn.transform.position);
                break;
            }
        }
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerSpawn();
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
