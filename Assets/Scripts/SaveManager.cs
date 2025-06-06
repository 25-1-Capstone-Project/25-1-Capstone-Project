using UnityEngine;
using Firebase.Database;
using Firebase;
using Firebase.Extensions;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public enum NetworkState { Online, Offline }
    public NetworkState CurrentNetworkState { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Awake에서는 바로 Firebase 접근 안함
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initialize 메서드는 public으로 선언
    public void Initialize()
    {
        //CheckNetwork();

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase 초기화 성공");
                CheckNetwork(); // 초기화 후에만 네트워크 확인 및 저장
            }
            else
            {
                Debug.LogError("Firebase 초기화 실패: " + task.Result);
            }
        });
    }

    void CheckNetwork()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            CurrentNetworkState = NetworkState.Offline;
        }
        else
        {
            CurrentNetworkState = NetworkState.Online;
        }

        Debug.Log("현재 네트워크 상태: " + CurrentNetworkState);
        SaveNetworkStateToFirebase();
    }

    void SaveNetworkStateToFirebase()
    {
        string userId = SystemInfo.deviceUniqueIdentifier;
        string path = "users/" + userId + "/networkState";

        FirebaseDatabase.DefaultInstance
            .GetReference(path)
            .SetValueAsync(CurrentNetworkState.ToString())
            .ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    Debug.Log("Firebase에 네트워크 상태 저장 완료");
                }
                else
                {
                    Debug.LogError("Firebase에 상태 저장 실패: " + task.Exception);
                }
            });
    }
}
