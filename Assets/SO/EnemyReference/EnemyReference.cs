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
        int typeIndex = (int)eDungeonType;
        EnemyData[] enemydatas = enemyPrefabSet[typeIndex].enemyDatas;
        int level = GameManager.Instance.CurrentLevel;
        return enemydatas[Random.Range(0, level)];
    }

    public BossData GetRandomBossData()
    {
        int floor = GameManager.Instance.CurrentLevel;

        BossData[] bossDatas = enemyPrefabSet[floor].bossDatas;
        return bossDatas[Random.Range(0, bossDatas.Length)];


    }

}