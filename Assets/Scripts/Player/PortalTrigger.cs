using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
/// <summary>
/// 씬 전환을 위한 트리거입니다.
/// 플레이어가 이 트리거에 닿으면 지정된 씬으로 전환됩니다.
/// 씬 전환 시 플레이어의 스폰 포인트를 지정하는 PlayerSpawnPoint와 ID를 맞춰야 합니다.
/// </summary>
public class ScenePortalTrigger : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private int targetSpawnPointID;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;
        if (!other.CompareTag("Player")) return;

        isTransitioning = true;
        // 플레이어 입력 차단
        SceneTransitionCarrier.Instance.TransitionScene(targetSceneName,targetSpawnPointID);
    }

   
}