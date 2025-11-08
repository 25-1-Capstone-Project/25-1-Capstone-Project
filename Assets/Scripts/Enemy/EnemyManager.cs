using System.Collections;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    int spawnedEnemy;

    public GameObject enemyPrefab;

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
            StageManager.Instance.GetCurrentRoom().ClearWave();
        }
    }

    public void EnemySpawn(EnemyDataBase enemyData, Vector2 spawnPos)
    {

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemyObj.transform.localScale *= enemyData.sizeMagnification;
        EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
        enemy.SetEnemyData(enemyData);
        enemy.Init();
        spawnedEnemy++;
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
