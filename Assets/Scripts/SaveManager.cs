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
