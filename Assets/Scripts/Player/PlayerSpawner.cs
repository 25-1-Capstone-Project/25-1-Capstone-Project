using UnityEngine;

/// <summary>
/// 씬 전환 시 플레이어의 스폰 포인트를 관리하는 스크립트입니다.
/// 씬 전환 시 PlayerSpawnPoint와 ID를 맞춰야 합니다.
/// 
/// </summary>
public class PlayerSpawnManager : MonoBehaviour
{
    void Start()
    {
        if (SceneTransitionCarrier.Instance == null) return;

        string spawnID = SceneTransitionCarrier.Instance.spawnPointID;
        if (string.IsNullOrEmpty(spawnID)) return;

        var spawns = FindObjectsOfType<PlayerSpawnPoint>();
        foreach (var spawn in spawns)
        {
            if (spawn.spawnPointID == spawnID)
            {
                var player = PlayerScript.Instance;
                player.transform.position = spawn.transform.position;
                break;
            }
        }
    }
}
