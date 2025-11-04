using System.Collections;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    int spawnedEnemy;
    public float spawnDelay = 2f;
    public EnemyReference enemyReference;
    public GameObject enemyPrefab;
    public int curLevel = 1;
    Transform enemySpawnParent;
    Transform[] enemySpawnPoints;
    public EnemyAttackPattern[] CloseEnemyAttackPatterns;
    public EnemyAttackPattern[] LongenemyAttackPatterns;
    public EnemyAttackPattern[] SpecialEnemyAttackPatterns;
    protected override void Awake()
    {
        base.Awake();
        spawnedEnemy = 0;
    }

    public void InitSpawnedEnemy()
    {
        spawnedEnemy = 0;
    }
    public void InitEnemySpawnPoints()
    {
        enemySpawnParent = GameObject.FindWithTag("EnemySpawnPointParent").transform;
        enemySpawnPoints = enemySpawnParent.GetComponentsInChildren<Transform>();
    }
    public void KillEnemy()
    {
        spawnedEnemy--;

        if (spawnedEnemy <= 0)
        {
            spawnedEnemy = 0;
            MapManager.Instance.GetCurrentRoom().ClearRound();
        }

    }

    public void EnemySpawn()
    {
        enemySpawnParent.position = PlayerScript.Instance.GetPlayerTransform().position;
        EnemyData enemyData = enemyReference.GetRandomEnemyData();
        GameObject enemyObj = Instantiate(enemyPrefab, enemySpawnPoints[Random.Range(1, enemySpawnPoints.Length)].position, Quaternion.identity);
        enemyObj.transform.localScale *= enemyData.sizeMagnification;
        EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
        enemy.SetEnemyData(enemyData);
        enemy.Init();
        spawnedEnemy++;
    }
    public void StartEnemySpawnRoutine()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }
    public IEnumerator SpawnEnemyRoutine()
    {
        while (!PlayerScript.Instance.GetIsDead())
        {
            EnemySpawn();
            yield return new WaitForSeconds(spawnDelay);
        }

    }
    // public void BossSpawn(Vector2 spawnPos)
    // {
    //     BossData bossData = enemyReference.GetRandomBossData();
    //     GameObject bossObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    //     Boss boss = bossObj.GetComponent<Boss>();
    //     boss.SetEnemyData(bossData);
    //     boss.Init();
    //     spawnedEnemy++;
    // }
}
