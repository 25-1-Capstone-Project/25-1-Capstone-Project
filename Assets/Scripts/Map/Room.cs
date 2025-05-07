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
    public Transform[] enemySpawnPointsT; // 스폰포인트

    List<GameObject> PortalPointObj = new List<GameObject>(); // 적 프리팹

    public void InitRoom()
    {
        // 방 초기화 로직을 여기에 추가하세요.
        IsRoomCleared = false;
        if(enemySpawnParentObject!=null)
        enemySpawnPointsT = enemySpawnParentObject.GetComponentsInChildren<Transform>();
    }
    void Awake()
    {
        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        if (!isRoomCleared)
            SpawnEnemies();
    }
    public void AddPortalPointObj(GameObject obj)
    {
        PortalPointObj.Add(obj);
    }

    public void SpawnEnemies()
    {
        foreach (Transform spawnPoint in enemySpawnPointsT)
        {
                EnemyManager.Instance.EnemySpawn(spawnPoint.position);
        }
    }

    public void ClearRoom()
    {
        IsRoomCleared = true;
    }







}
