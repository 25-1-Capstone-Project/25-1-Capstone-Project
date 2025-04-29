using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enmey/Enemy_SwordSlash")]
public class Enemy_SwordSlash : EnemyAttackPattern
{
    public override IEnumerator Execute(Enemy enemy)
    {
        enemy.IsAttacking = true;
        enemy.GetAnimatorController().PlayAttack();
        Vector2 attackDir = (PlayerScript.Instance.transform.position - enemy.transform.position).normalized;
        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        enemy.FlashSprite(Color.blue, delay);
        yield return new WaitForSeconds(delay);
        ObjectPooler.Instance.SpawnFromPool("AttackSlashParticle", enemy.transform.position, Quaternion.Euler(0f, 0f, angle));
        Vector2 boxCenter = (Vector2)enemy.transform.position + attackDir * 0.5f;
        Vector2 boxSize = new Vector2(1f, 1f);

        // 단일 대상 판정
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, angle, LayerMask.GetMask("Player"));

        if (hit != null && hit.TryGetComponent<PlayerScript>(out var player))
        {
            player.TakeDamage(enemy.GetDamage(), attackDir, enemy); // 예시

        }
        yield return new WaitForSeconds(0.5f);
        enemy.IsAttacking = false;
    }


  
}
