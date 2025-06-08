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
                EnemyAttackBase enemyAttack = other.GetComponent<EnemyAttackBase>();
                PlayerScript.Instance.TakeDamage(enemyAttack);
                break;
        }


    }
}
