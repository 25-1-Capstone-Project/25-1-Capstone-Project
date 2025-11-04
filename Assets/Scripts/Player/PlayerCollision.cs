using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 충돌 처리
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {

        switch (other.tag)
        {
            case "EnemyAttack":
                EnemyAttackBase enemyAttack = other.GetComponent<EnemyAttackBase>();
                PlayerScript.Instance.TakeAttack(enemyAttack);
                break;
            case "Enemy":

                EnemyBase enemy = other.GetComponent<EnemyBase>();
                if (enemy.GetData().CanBodyDamage)
                    PlayerScript.Instance.TakeAttack(enemy);
                break;
        }


    }
}
