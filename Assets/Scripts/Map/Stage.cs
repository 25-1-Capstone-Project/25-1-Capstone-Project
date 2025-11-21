using System;
using System.Collections;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [Header("State")]
    [SerializeField] bool isRoomCleared = false;
    public string roomName;

    [Header("Editor Placement")]
    [Tooltip("이 룸을 시각적으로 배치할 때 스폰 마커를 담을 폴더(선택)")]
    public Transform markerRoot;


    [Header("Wave")]
    [SerializeField] int waveIndex = 0;

    public Wave[] waves;

    public void InitRoom()
    {
        isRoomCleared = false;
        waveIndex = 0;
        EnemyManager.Instance.InitSpawnedEnemy();
        StartWave();
    }

    public void StartWave()
    {
        if (isRoomCleared) return;
        if (waves == null || waves.Length == 0) return;
        if (waveIndex >= waves.Length) return;

        StartCoroutine(StartWaveRoutine());
    }

    IEnumerator StartWaveRoutine()
    {
        yield return null; // 한 프레임 대기
        var wave = waves[Mathf.Clamp(waveIndex, 0, waves.Length - 1)];
        if (wave.startDelay > 0) yield return new WaitForSeconds(wave.startDelay);

        foreach (var s in wave.spawns) { EnemyManager.Instance.EnemySpawn(s.enemyData, s.marker); }

    }


    public void ClearWave()
    {
        waveIndex++;
        if (waveIndex < waves.Length && waveIndex < (waves?.Length ?? 0))
        {
            StartCoroutine(NextWaveRoutine());
        }
        else
        {
            StartCoroutine(ClearRoom());
        }
    }

    IEnumerator NextWaveRoutine()
    {
        yield return new WaitForSeconds(1f);
        StartWave();
    }

    public IEnumerator ClearRoom()
    {
        GameManager.Instance.playerScript.StopAllCoroutines();
        GameManager.Instance.SetTimeScale(0.5f);
        ShaderManager.Instance.SetBleackScreen(true);
        GameManager.Instance.playerScript.SetActivePlayerInput(false);
        yield return new WaitForSecondsRealtime(1f);
        ShaderManager.Instance.SetBleackScreen(false);
        GameManager.Instance.playerScript.ClearSet();
        GameManager.Instance.SetTimeScale(1);

        UIManager.Instance.successUI.SetActiveDeadInfoPanel(true);
        isRoomCleared = true;
    }

}

[Serializable]
public class Wave
{
    public float startDelay = 0f;
    public EnemySpawn[] spawns;
}

[Serializable]
public class EnemySpawn
{
    [Tooltip("이 적 데이터의 Name으로 마커 라벨 자동 설정")]
    public EnemyDataBase enemyData;

    public EnemySpawnMarker marker;
}
