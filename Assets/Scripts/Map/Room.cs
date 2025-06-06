using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public Tilemap GroundTileMap; // 방 타일맵
    List<GameObject> PortalPointObj = new List<GameObject>(); 

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
