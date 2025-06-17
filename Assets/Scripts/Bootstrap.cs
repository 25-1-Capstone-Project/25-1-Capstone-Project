using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Extensions;
using System.Collections;

public class BootStrap : MonoBehaviour
{
    [SerializeField] string firstSceneName = "MainMenu";

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase 초기화 성공");

                if (SaveManager.Instance == null)
                {
                    SaveManager sm = gameObject.AddComponent<SaveManager>();
                    sm.Initialize();
                }
                else
                {
                    SaveManager.Instance.Initialize();
                }


            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {dependencyStatus}");
            }
        });
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60; // FPS 설정
        SceneManager.LoadScene(firstSceneName);

    }
}
