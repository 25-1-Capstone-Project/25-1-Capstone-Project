using UnityEngine;

public class BossEnterTrigger : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.StopBGM();
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            EnemyManager.Instance.BossSpawnEvent();
        }
    }
}
