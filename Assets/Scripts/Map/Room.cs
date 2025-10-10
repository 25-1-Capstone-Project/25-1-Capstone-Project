using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{

    bool isRoomCleared = false;
    
    [SerializeField] public Transform enemySpawnParentObject; // 스폰포인트 부모 오브젝트
    public Transform[] enemySpawnPointsT; // 스폰포인트
    public Tilemap GroundTileMap; // 방 타일맵


    public void InitRoom()
    {
        // 부모(자기 자신)는 제외하고 자식들만 할당
        if (enemySpawnParentObject != null)
        {
            List<Transform> spawnPoints = new List<Transform>();
            foreach (Transform t in enemySpawnParentObject.GetComponentsInChildren<Transform>())
            {
                if (t != enemySpawnParentObject)
                    spawnPoints.Add(t);
            }
            enemySpawnPointsT = spawnPoints.ToArray();
        }
        // 방 초기화 로직을 여기에 추가하세요.
        isRoomCleared = false;
        if(enemySpawnParentObject!=null)
        enemySpawnPointsT = enemySpawnParentObject.GetComponentsInChildren<Transform>();
    }
    void Awake()
    {
        InitRoom();
    }
   

    public void SpawnEnemies()
    {Debug.Log("SpawnEnemies");
        if (isRoomCleared) return;
        Debug.Log("SpawnEnemies1");
        foreach (Transform spawnPoint in enemySpawnPointsT)
        {
             Debug.Log("SpawnEnemies2");
                EnemyManager.Instance.EnemySpawn(spawnPoint.position);
        }
    }

    public void ClearRoom()
    {
        isRoomCleared = true;
    }







}
