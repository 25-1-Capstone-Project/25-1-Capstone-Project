using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public Transform spawnPosParent;
    private Transform[] spawnPointsT;
    public GameObject enemyPrefab;
    public EnemyAttackPattern[] commonEnemyAttackPatterns;
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

        spawnPointsT = spawnPosParent.GetComponentsInChildren<Transform>();
    }
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (!PlayerScript.instance.GetIsDead())
        {
            Spawn();
            yield return new WaitForSeconds(10);
        }
    }
    void Spawn()
    {
        int index = Random.Range(0, spawnPointsT.Length);
        Instantiate(enemyPrefab, spawnPointsT[index].position, Quaternion.identity);
    }

}
