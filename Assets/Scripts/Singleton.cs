using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object lockObj = new();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    instance = FindFirstObjectByType<T>();

                    if (instance == null)
                    {
                        Debug.LogError($"Singleton of type {typeof(T)} is missing in the scene.");
                    }
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}