using System.Collections;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    int spawnedEnemy;

    public GameObject enemyPrefab;
    public EnemyReference enemyReference;
    public EnemyAttackPattern[] CloseEnemyAttackPatterns;
    public EnemyAttackPattern[] LongenemyAttackPatterns;
    protected override void Awake()
    {
        base.Awake();
        spawnedEnemy = 0;
    }

    public void InitSpawnedEnemy()
    {
        spawnedEnemy = 0;
    }
    public void KillEnemy()
    {
        spawnedEnemy--;

        if (spawnedEnemy <= 0)
        {
            spawnedEnemy = 0;
            MapManager.Instance.GetCurrentRoom().ClearRoom();
        }

    }

    public void EnemySpawn(Vector2 spawnPos)
    {
        EnemyData enemyData = enemyReference.GetRandomEnemyData();
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
        enemy.SetEnemyData(enemyData);
        enemy.Init();
        spawnedEnemy++;
    }
    
    public void BossSpawn(Vector2 spawnPos)
    {
        BossData bossData = enemyReference.GetRandomBossData();
        GameObject bossObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        Boss boss = bossObj.GetComponent<Boss>();
        boss.SetEnemyData(bossData);
        boss.Init();
        spawnedEnemy++;
    }
}
