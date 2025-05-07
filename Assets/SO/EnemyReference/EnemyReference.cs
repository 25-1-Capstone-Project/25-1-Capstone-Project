using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyReference", menuName = "Enemy/EnemyReference")]
public class EnemyReference : ScriptableObject
{
    public EDungeonType eDungeonType;

    [System.Serializable]
    public class EnemyFloorSet
    {
        public EnemyData[] enemyDatas;
    }

    [SerializeField] EnemyFloorSet[] enemyPrefabSet;

    public EnemyData GetRandomEnemyPrefab()
    {
        int floor = GameManager.Instance.CurrentDungeonFloor;
        if (floor < enemyPrefabSet.Length && enemyPrefabSet[floor].enemyDatas.Length > 0)
        {
            var prefabs = enemyPrefabSet[floor].enemyDatas;
            return prefabs[Random.Range(0, prefabs.Length)];
        }

        Debug.LogWarning("No enemy prefab available for floor: " + floor);
        return null;
    }
}