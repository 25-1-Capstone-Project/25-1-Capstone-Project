using UnityEngine;

public class SceneTransitionCarrier : Singleton<SceneTransitionCarrier>
{

    public string targetSceneName;
    public string spawnPointID;

    protected override void Awake()
    {
        base.Awake();
    }
}
