using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Extensions;


public class BootStrap : MonoBehaviour
{
    [SerializeField] string firstSceneName = "MainMenu";

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
    
        });
    }

    private void Start()
    {
       // QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60; // FPS 설정
        SceneManager.LoadScene(firstSceneName);

    }
}
