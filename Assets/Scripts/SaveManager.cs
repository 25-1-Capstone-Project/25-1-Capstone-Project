using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public enum NetworkState
    {
        Online,
        Offline
    }

    public NetworkState CurrentNetworkState { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CheckNetwork();
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
    }
}
