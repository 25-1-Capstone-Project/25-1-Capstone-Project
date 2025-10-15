
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{

    bool isRoomCleared = false;

    [SerializeField] private Transform enemySpawnParentObject; // 스폰포인트 부모 오브젝트
    [SerializeField] private Transform[] enemySpawnPointsT; // 스폰포인트
    [SerializeField] private Animator[] enemySpawnAnim; // 스폰포인트
    public Tilemap GroundTileMap; // 방 타일맵
    int round = 0;
    [SerializeField] private int maxRound = 3;

    public void InitRoom()
    {
        // 방 초기화 로직을 여기에 추가하세요.
        isRoomCleared = false;

        if (enemySpawnParentObject != null)
        {
            enemySpawnPointsT = enemySpawnParentObject.GetComponentsInChildren<Transform>().ToList().Where(t => t != enemySpawnParentObject).ToArray();
            enemySpawnAnim = enemySpawnParentObject.GetComponentsInChildren<Animator>();
        }
    }
    


    public void SpawnEnemies()
    {
        if (isRoomCleared) return;

        for (int i = 0; i < enemySpawnPointsT.Length; i++)
        {
            EnemyManager.Instance.EnemySpawn(enemySpawnPointsT[i].position);
            enemySpawnAnim[i].SetTrigger("Spawn");
        }
    }


    public void ClearRound()
    {
        round++;
        if (round < maxRound)
        {
            StartCoroutine(ClearRoutine());
        }
        else
        {
            ClearRoom();
        }
    }

    IEnumerator ClearRoutine()
    {
        yield return new WaitForSeconds(1f);
        SpawnEnemies();
    }
    public void ClearRoom()
    {
        isRoomCleared = true;
    }







}
