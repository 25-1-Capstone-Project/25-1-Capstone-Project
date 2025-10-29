using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_NinjaAttack")]
public class Enemy_NinjaAttack : EnemyAttackPattern
{
    public float attackDistance;
    public override IEnumerator Execute(EnemyBase enemy)
    {
        enemy.InitStamina();
        for (int i = 0; i < attackCount; i++)
        {
            Vector2 offset = Random.Range(0, 3) switch
            {
                0 => new Vector2(attackRange, 0),
                1 => new Vector2(-attackRange, 0),
                2 => new Vector2(0, attackRange),
                _ => new Vector2(0, -attackRange),
            };
            Vector2 playerPos = PlayerScript.Instance.GetPlayerTransform().position;
            Vector2 direction = offset.normalized;
            Vector2 pos;
            // Raycast 발사
            RaycastHit2D point = Physics2D.Raycast(playerPos, direction, attackRange, LayerMask.GetMask("Wall"));

            // 충돌 판정
            if (point.collider != null)
            {
                // 벽에 막힘
                pos = point.point - direction * 0.1f; // 충돌 지점
            }
            else
            {
                // 벽에 안 막힘 → 플레이어 기준 offset 위치로
                pos = playerPos + offset;
            }


            enemy.transform.position = pos;
            enemy.SpriteFlip();
            enemy.GetRigidbody().linearVelocity = Vector2.zero;


            Vector2 dir = (PlayerScript.Instance.transform.position - enemy.transform.position).normalized;
            Vector2 startPos = enemy.transform.position;
            Vector2 endPos = startPos + dir * attackDistance;
            enemy.GetAnimatorController().PlayAttack();
            enemy.enemyShaderController.OnOutline();

            //공격 준비
            float time = 0f;
            while (time < attackChargeSec)
            {
                float t = time / attackChargeSec;
                // spearEffect.startWidth = spearEffect.endWidth = effectWidth * (1 - t);
                time += Time.deltaTime;
                yield return null;
            }

            // spearEffect.gameObject.SetActive(false);
            time = 0f;
            bool hasDealtDamage = false;

            //공격 
            enemy.gameObject.layer = LayerMask.NameToLayer("EnemyAttack");
            while (time < attackDuration)
            {
                float t = time / attackDuration;
                enemy.GetRigidbody().MovePosition(Vector3.Lerp(startPos, endPos, t));

                if (!hasDealtDamage)
                {
                    Collider2D hit = Physics2D.OverlapCircle((Vector2)enemy.transform.position, 0.3f, LayerMask.GetMask("Player", "PlayerDash"));


                    if (hit != null && hit.CompareTag("Player"))
                    {
                        PlayerScript.Instance.TakeAttack(enemy);
                        hasDealtDamage = true;
                    }

                }

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            enemy.gameObject.layer = LayerMask.NameToLayer("Enemy");
            enemy.enemyShaderController.OffOutline();
            yield return new WaitForSeconds(attackPostDelay);
        }
        yield return new WaitForSeconds(1f);
    }
}
