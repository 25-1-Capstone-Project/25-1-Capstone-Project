using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_SwordSlash")]
public class Enemy_SwordSlash : EnemyAttackPattern
{

    public override IEnumerator Execute(EnemyBase enemy)
    {

        enemy.GetAnimatorController().PlayAttack();
        yield return new WaitForEndOfFrame();

        enemy.enemyShaderController.OnOutline();

        Vector2 attackDir = (GameManager.Instance.playerScript.transform.position - enemy.transform.position).normalized;
        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

        Vector2 boxSize = new Vector2(attackRange, 1);

        // 상자 중심을 공격 방향의 반만큼 이동(상자가 적절히 앞에 위치하도록)
        Vector2 boxCenter = (Vector2)enemy.transform.position + attackDir * (attackRange * 0.6f);

        yield return new WaitForSeconds(attackChargeSec);
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, angle, LayerMask.GetMask("Player"));

        // 디버그: 회전된 상자 모양을 씬에 그림 (에디터에서 확인용)
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);
        Vector2 right = rot * Vector2.right * (boxSize.x * 0.5f);
        Vector2 up = rot * Vector2.up * (boxSize.y * 0.5f);
        Vector2 p1 = boxCenter - right - up;
        Vector2 p2 = boxCenter + right - up;
        Vector2 p3 = boxCenter + right + up;
        Vector2 p4 = boxCenter - right + up;
        Debug.DrawLine(p1, p2, Color.red, 0.1f);
        Debug.DrawLine(p2, p3, Color.red, 0.1f);
        Debug.DrawLine(p3, p4, Color.red, 0.1f);
        Debug.DrawLine(p4, p1, Color.red, 0.1f);

        if (hit != null && hit.TryGetComponent<PlayerScript>(out var player))
        {
            player.TakeAttack(enemy); // 예시

        }
        yield return new WaitForSeconds(attackDuration);
        enemy.enemyShaderController.OffOutline();
        yield return new WaitForSeconds(attackPostDelay);


    }



}
