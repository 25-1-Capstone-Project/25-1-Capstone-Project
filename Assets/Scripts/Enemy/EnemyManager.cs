using System.Collections;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{

    public Transform spawnPosParent;
    private Transform[] spawnPointsT;
    public GameObject enemyPrefab;
    public EnemyAttackPattern[] commonEnemyAttackPatterns;
    protected override void Awake()
    {
        base.Awake();
        spawnPointsT = spawnPosParent.GetComponentsInChildren<Transform>();
    }


    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (!PlayerScript.Instance.GetIsDead())
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
