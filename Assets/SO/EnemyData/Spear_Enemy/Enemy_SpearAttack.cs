using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_SpearAttack")]
public class Enemy_SpearAttack : EnemyAttackPattern
{
    public float attackDistance;
    public float attackChargeSec;
    public float attackDuration;
    public float attackPostDelay;
    public float effectWidth = 0.4f;
    public Vector2 hitboxSize = new Vector2(1f, 1f);

    public override IEnumerator Execute(Enemy enemy)
    {
        enemy.GetRigidbody().linearVelocity = Vector2.zero;
        enemy.FlashSprite(Color.blue, attackChargeSec);

        Vector2 dir = (PlayerScript.Instance.transform.position - enemy.transform.position).normalized;
        Vector2 startPos = enemy.transform.position;
        Vector2 endPos = startPos + dir * attackDistance;

        //effect visualization
        LineRenderer spearEffect = EffectPooler.Instance.SpawnFromPool<LineRenderer>("AttackSpearEffect");
        spearEffect.useWorldSpace = true;
        spearEffect.SetPosition(0, startPos);
        spearEffect.SetPosition(1, endPos);
        spearEffect.startWidth = effectWidth;
        spearEffect.endWidth = effectWidth;
        enemy.CurrentSpearIndicator = spearEffect;
        try
        {
            float time = 0f;
            while (time < attackChargeSec)
            {
                float t = time / attackChargeSec;
                spearEffect.startWidth = spearEffect.endWidth = effectWidth * (1 - t);
                time += Time.deltaTime;
                yield return null;
            }

            spearEffect.gameObject.SetActive(false);
            time = 0f;
            bool hasDealtDamage = false;

            while (time < attackDuration)
            {
                float t = time / attackDuration;
                enemy.GetRigidbody().MovePosition(Vector3.Lerp(startPos, endPos, t));

                if (!hasDealtDamage)
                {
                    Vector2 boxCenter = (Vector2)enemy.transform.position + dir * hitboxSize.x * 0.5f;
                    Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, hitboxSize, 0f, LayerMask.GetMask("Player"));

                    foreach (var hit in hits)
                    {
                        if (hit.CompareTag("Player"))
                        {
                            PlayerScript.Instance.TakeDamage(enemy.GetDamage(), dir, enemy);
                            hasDealtDamage = true;
                            break;
                        }
                    }
                }

                time += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(attackPostDelay);
        }
        finally
        {
            enemy.ClearAttackEffect();
        }
    }
}
