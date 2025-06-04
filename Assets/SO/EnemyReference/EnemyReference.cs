using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyReference", menuName = "Enemy/EnemyReference")]
public class EnemyReference : ScriptableObject
{
    public EDungeonType eDungeonType;

    [System.Serializable]
    public class FloorSpawnSet
    {
        public EnemyData[] enemyDatas;
        public BossData[] bossDatas;
    }

    [SerializeField] FloorSpawnSet[] enemyPrefabSet;

    public EnemyData GetRandomEnemyData()
    {
        int floor = GameManager.Instance.CurrentDungeonFloor;

        EnemyData[] enemydatas = enemyPrefabSet[floor].enemyDatas;
        return enemydatas[Random.Range(0, enemydatas.Length)];

    }

    public BossData GetRandomBossData()
    {
        int floor = GameManager.Instance.CurrentDungeonFloor;

       BossData[] bossDatas = enemyPrefabSet[floor].bossDatas;
        return bossDatas[Random.Range(0, bossDatas.Length)];


    }

}