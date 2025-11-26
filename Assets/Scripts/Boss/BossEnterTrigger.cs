using UnityEngine;
using UnityEngine.UIElements;

public class BossEnterTrigger : MonoBehaviour
{
    [SerializeField] BoxCollider2D door;
    [SerializeField] BoxCollider2D bossTrigger;
    [SerializeField] Animator animator;
    void Start()
    {
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySFX("OpenStoneDoor");
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            door.enabled = true;
            animator.SetTrigger("Close");
            EnemyManager.Instance.BossSpawnEvent();
            bossTrigger.enabled=false;
        }
    }
}
