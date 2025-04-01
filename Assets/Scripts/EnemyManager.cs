using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

     void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public EnemyAttackPattern[] commonEnemyAttackPatterns;
}
