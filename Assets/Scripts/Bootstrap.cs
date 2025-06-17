// BootStrap.cs
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class BootStrap : MonoBehaviour
{
    [SerializeField] string firstSceneName = "MainMenu";
    public AugmentRecommender recommender;

    void Awake()
    {
        recommender.gameObject.SetActive(false); // 초기에는 꺼둠

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase 초기화 성공");

                // SaveManager 초기화
                if (SaveManager.Instance == null)
                {
                    gameObject.AddComponent<SaveManager>();
                }

                // 추천 시스템 실행
                recommender.Run(); // 👉 직접 실행 메서드로 호출
                SceneManager.LoadScene(firstSceneName);
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {dependencyStatus}");
            }
        });
    }
}

