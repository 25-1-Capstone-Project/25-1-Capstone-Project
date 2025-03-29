using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 충돌 처리
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {

        switch (other.tag)
        {
            case "Enemy":
             
                Vector3 enemyDirection = (transform.position - other.transform.position).normalized;
                GameManager.instance.player.EnemyContact(enemyDirection);
                break;
        }


    }
}
