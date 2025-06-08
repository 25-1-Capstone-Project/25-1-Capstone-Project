using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_SpearAttack")]
public class Enemy_SpearAttack : EnemyAttackPattern
{
    public float attackDistance;
    public float effectWidth = 0.4f;

    public override IEnumerator Execute(EnemyBase enemy)
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
        enemy.GetAnimatorController().PlayAttack();
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
        enemy.gameObject.layer = LayerMask.NameToLayer("EnemyAttack");
        while (time < attackDuration)
        {
            float t = time / attackDuration;
            enemy.GetRigidbody().MovePosition(Vector3.Lerp(startPos, endPos, t));

            if (!hasDealtDamage)
            {
                Collider2D hit = Physics2D.OverlapCircle((Vector2)enemy.transform.position, 0.3f, LayerMask.GetMask("Player"));


                if (hit != null && hit.CompareTag("Player"))
                {
                    PlayerScript.Instance.TakeDamage(enemy);
                    hasDealtDamage = true;
                }

            }

            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        enemy.gameObject.layer = LayerMask.NameToLayer("Enemy");
        yield return new WaitForSeconds(attackPostDelay);


    }
}
