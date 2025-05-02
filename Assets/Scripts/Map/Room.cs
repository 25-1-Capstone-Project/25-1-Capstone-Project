using System;
using UnityEngine;

public class Room : MonoBehaviour
{

    bool isRoomCleared = false;
    public Transform[] enemySpawnPointObject; // 적들

    public void SpawnEnemies()
    {
        foreach (Transform spawnPoint in enemySpawnPointObject)
        {
            // 적 스폰 로직을 여기에 추가하세요.
            // 예를 들어, Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
        }
    }
    public void ClearRoom()
    {
        isRoomCleared = true;
    }
    
}
