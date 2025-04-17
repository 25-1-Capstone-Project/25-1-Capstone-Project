using UnityEngine;

public class SceneTransitionCarrier : MonoBehaviour
{
    public static SceneTransitionCarrier instance;

    public string targetSceneName;
    public string spawnPointID;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
     
    }
}
