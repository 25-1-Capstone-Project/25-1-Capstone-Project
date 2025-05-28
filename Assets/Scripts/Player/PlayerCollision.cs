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
            case "EnemyAttack":
                EnemyAttack enemyAttack = other.GetComponent<EnemyAttack>();
                other.gameObject.SetActive(false);
                PlayerScript.Instance.TakeDamage(enemyAttack);

                break;
        }


    }
}
