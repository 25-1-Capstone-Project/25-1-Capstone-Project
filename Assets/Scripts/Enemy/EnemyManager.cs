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
        EnemyData enemyData = enemyReference.GetRandomEnemyPrefab();
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.SetEnemyData(enemyData);
        enemy.EnemyInit();
        spawnedEnemy++;
    }

}
