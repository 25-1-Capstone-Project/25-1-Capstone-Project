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
    [SerializeField] public Transform enemySpawnParentObject; // 스폰포인트 부모 오브젝트
    public Transform[] enemySpawnPointObject; // 스폰포인트

    List<GameObject> PortalPointObj = new List<GameObject>(); // 적 프리팹

    public void InitRoom()
    {
        // 방 초기화 로직을 여기에 추가하세요.

        IsRoomCleared = false;
       // enemySpawnPointObject = enemySpawnParentObject.GetComponentsInChildren<Transform>();
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
