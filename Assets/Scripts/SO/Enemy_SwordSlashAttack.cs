using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_SwordSlash")]
public class Enemy_SwordSlash : EnemyAttackPattern
{

    public override IEnumerator Execute(EnemyBase enemy)
    {
        enemy.IsAttacking = true;
        enemy.GetAnimatorController().PlayAttack();
        yield return new WaitForEndOfFrame();

        enemy.enemyShaderController.OnOutline();

        yield return new WaitForSeconds(attackChargeSec);
        // enemy.GetAnimatorController().FreezeFrame(true);
        // yield return new WaitForSeconds(0.001f);
        // enemy.GetAnimatorController().FreezeFrame(false);
        Vector2 attackDir = (GameManager.Instance.playerScript.transform.position - enemy.transform.position).normalized;
        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
      //  EffectPooler.Instance.SpawnFromPool("AttackSlashParticle", enemy.transform.position, Quaternion.Euler(0f, 0f, angle));
        Vector2 boxCenter = (Vector2)enemy.transform.position + attackDir * attackRange;
        Vector2 boxSize = new Vector2(1f, 1f);

        // 단일 대상 판정
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, angle, LayerMask.GetMask("Player"));

        if (hit != null && hit.TryGetComponent<PlayerScript>(out var player))
        {
            player.TakeAttack(enemy); // 예시

        }
        yield return new WaitForSeconds(attackDuration);
        enemy.enemyShaderController.OffOutline();
        yield return new WaitForSeconds(attackPostDelay);
        enemy.IsAttacking = false;

    }



}
