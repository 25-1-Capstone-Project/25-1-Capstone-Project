using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    bool isRoomCleared = false;
    bool IsRoomCleared
    {
        get { return isRoomCleared; }
        set
        {
            isRoomCleared = value;
            if (isRoomCleared)
            {
                foreach (GameObject obj in PortalPointObj)
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject obj in PortalPointObj)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
    public Transform[] enemySpawnPointObject; // 적들
    List<GameObject> PortalPointObj = new List<GameObject>(); // 적 프리팹

    public void InitRoom()
    {
        // 방 초기화 로직을 여기에 추가하세요.
        // 예를 들어, 적 스폰 포인트를 초기화하거나, 방의 상태를 설정하는 등의 작업을 수행할 수 있습니다.
        IsRoomCleared = false;
    }


    public void AddPortalPointObj(GameObject obj)
    {
        PortalPointObj.Add(obj);
    }

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
        IsRoomCleared = !IsRoomCleared;
    }

}
