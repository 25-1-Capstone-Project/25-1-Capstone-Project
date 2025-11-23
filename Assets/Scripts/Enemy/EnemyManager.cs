
using System.Collections;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    int spawnedEnemy;

    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    protected override void Awake()
    {
        base.Awake();
        spawnedEnemy = 0;
    }

    public void InitSpawnedEnemy()
    {
        spawnedEnemy = 0;
    }
    public void KillEnemy()
    {

        spawnedEnemy--;

        if (spawnedEnemy <= 0)
        {
            spawnedEnemy = 0;
            StageManager.Instance.GetCurrentStage().ClearWave();
        }
    }

    public void EnemySpawn(EnemyDataBase enemyData, EnemySpawnMarker maker)
    {
        GameObject enemyObj = Instantiate(enemyPrefab, maker.transform.position, Quaternion.identity);
        enemyObj.transform.localScale *= enemyData.sizeMagnification;
        EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
        enemy.SetEnemyData(enemyData);
        enemy.Init();

        Animator anim = maker.GetComponent<Animator>();
        anim.SetTrigger("Spawn");
        spawnedEnemy++;
    }
    IEnumerator BossSpawnRoutine()
    {
        GameManager.Instance.playerScript.SetActivePlayerInput(false);

        // 카메라 추적 중단
        CameraManager.Instance.SetActiveCineCam(false);

        Transform player = GameManager.Instance.playerScript.GetPlayerTransform();
        Vector3 playerPos = player.position;
        EnemySpawn spawn = StageManager.Instance.GetCurrentStage().waves[0].spawns[0];
        // 1) 연출용 카메라 이동 (위로)
        Vector3 bossViewPos = spawn.marker.transform.position; // 카메라를 위로 6만큼 이동
        yield return StartCoroutine(CameraManager.Instance.LerpCameraPosition(bossViewPos));

        // 2) 연출 대기
        yield return new WaitForSeconds(1.0f);

        // 3) 보스 스폰
        Boss boss = SpawnBoss(spawn);

        yield return new WaitForSeconds(1f);
        // 4) 보스 등장 연출 대기
        yield return new WaitForSeconds(boss.GetAnimatorController().GetAnimator().GetCurrentAnimatorClipInfo(0)[0].clip.length);

        // 5) 카메라 원래 플레이어에게 복귀
        yield return StartCoroutine(CameraManager.Instance.LerpCameraPosition(playerPos));

        // 6) 카메라 Follow 다시 활성화
        CameraManager.Instance.SetActiveCineCam(true);

        GameManager.Instance.playerScript.SetActivePlayerInput(true);
    }

    private Boss SpawnBoss(EnemySpawn spawn)
    {
        GameObject enemyObj = Instantiate(
            bossPrefab,
            spawn.marker.transform.position,
            Quaternion.identity
        );

        enemyObj.transform.localScale *= spawn.enemyData.sizeMagnification;

        EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
        enemy.SetEnemyData(spawn.enemyData);
        enemy.Init();

        Animator anim = spawn.marker.GetComponent<Animator>();
        anim.SetTrigger("Spawn");

        spawnedEnemy++;
        return (Boss)enemy;
    }
    private void BossSpawn()
    {
        EnemySpawn spawn = StageManager.Instance.GetCurrentStage().waves[0].spawns[0];
        GameObject enemyObj = Instantiate(bossPrefab, spawn.marker.transform.position, Quaternion.identity);
        enemyObj.transform.localScale *= spawn.enemyData.sizeMagnification;
        EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
        enemy.SetEnemyData(spawn.enemyData);
        enemy.Init();
        Animator anim = spawn.marker.GetComponent<Animator>();
        anim.SetTrigger("Spawn");
        spawnedEnemy++;
    }

    public void BossSpawnEvent()
    {
        AudioManager.Instance.PlayBGM("Stage6Boss");
        StartCoroutine(BossSpawnRoutine());

    }
    public void ClearAllEnemies()
    {
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        spawnedEnemy = 0;
    }
    // public void BossSpawn(Vector2 spawnPos)
    // {
    //     BossData bossData = enemyReference.GetRandomBossData();
    //     GameObject bossObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    //     Boss boss = bossObj.GetComponent<Boss>();
    //     boss.SetEnemyData(bossData);
    //     boss.Init();
    //     spawnedEnemy++;
    // }
}
