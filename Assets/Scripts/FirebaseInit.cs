using UnityEngine;
using Firebase;
using Firebase.Auth;
using System;

public class FirebaseInit : MonoBehaviour
{
    public static FirebaseAuth auth;
    public Action OnFirebaseInitialized;

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase 초기화 완료");
                OnFirebaseInitialized?.Invoke();
            }
            else
            {
                Debug.LogError("Firebase 초기화 실패: " + dependencyStatus);
            }
        });
    }
}
