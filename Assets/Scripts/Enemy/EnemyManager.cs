using System.Collections;
using UnityEditor.Animations;
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

    public void EnemySpawn(EnemyDataBase enemyData, EnemySpawnMarker maker)
    {
        GameObject enemyObj = Instantiate(enemyPrefab, maker.transform.position, Quaternion.identity);
        enemyObj.transform.localScale *= enemyData.sizeMagnification;
        EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
        enemy.SetEnemyData(enemyData);
        enemy.Init();
    
        Animator anim = maker.GetComponent<Animator>();
        anim.SetTrigger("Spawn");
        spawnedEnemy++;
    }

    public void ClearAllEnemies()
    {
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        spawnedEnemy = 0;
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
