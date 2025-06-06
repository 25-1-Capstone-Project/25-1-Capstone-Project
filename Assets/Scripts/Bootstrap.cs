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

                StartCoroutine(LoadNextScene());
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {dependencyStatus}");
            }
        });
    }

    IEnumerator LoadNextScene()
    {
        yield return null;
        SceneManager.LoadScene(firstSceneName);
    }
}
