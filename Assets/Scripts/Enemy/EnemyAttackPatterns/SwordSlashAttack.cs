using UnityEngine;
using System.Collections;
using UnityEditor;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/SwordSlash")]
public class SwordSlashAttack : EnemyAttackPattern
{
    public int damage = 7;
    public float delay = 0.2f;
    public float range = 1f;

    public override IEnumerator Execute(Enemy enemy)
    {
        enemy.IsAttacking = true;

        // 공격 방향을 정확히 플레이어 기준으로
        Vector2 attackDir = (GameManager.instance.player.transform.position - enemy.transform.position).normalized;
        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

        yield return new WaitForSeconds(delay);
      //  enemy.GetAttackParticle().Play();
        //enemy.GetAttackParticleT().LookAt((Vector2)enemy.transform.position+ attackDir);
        Vector2 boxCenter = (Vector2)enemy.transform.position + attackDir * 0.5f;
        Vector2 boxSize = new Vector2(1f, 1f);

        // 단일 대상 판정
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, angle, LayerMask.GetMask("Player"));

        if (hit != null && hit.TryGetComponent<Player>(out var player))
        {
            player.TakeDamage(enemy.GetDamage(), attackDir, enemy); // 예시

        }
        yield return new WaitForSeconds(0.5f);
        enemy.IsAttacking = false;
    }



}
