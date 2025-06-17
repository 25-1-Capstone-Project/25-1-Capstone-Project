using UnityEngine;
using System.Collections;
[CreateAssetMenu(menuName = "Boss/AttackPattern/Skeleton/Boss_RushAttack")]
class Boss_RushAttack : EnemyAttackPattern
{
    [SerializeField]float attackDistance;
    [SerializeField] float effectWidth = 0.4f;
    [SerializeField] float attackCount = 3;
    public override IEnumerator Execute(EnemyBase boss)
    {
     for (int i = 0; i < attackCount; i++)
        {
            boss.GetRigidbody().linearVelocity = Vector2.zero;
            boss.enemyShaderController.OnOutline();

            Vector2 dir = (PlayerScript.Instance.transform.position - boss.transform.position).normalized;
            Vector2 startPos = boss.transform.position;
            Vector2 endPos = startPos + dir * attackDistance;

            //effect visualization
            LineRenderer spearEffect = EffectPooler.Instance.SpawnFromPool<LineRenderer>("AttackSpearEffect");
            spearEffect.useWorldSpace = true;
            spearEffect.SetPosition(0, startPos);
            spearEffect.SetPosition(1, endPos);
            spearEffect.startWidth = effectWidth;
            spearEffect.endWidth = effectWidth;
            boss.CurrentSpearIndicator = spearEffect;
            boss.GetAnimatorController().PlayAttack();
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
            boss.gameObject.layer = LayerMask.NameToLayer("EnemyAttack");
            while (time < attackDuration)
            {
                float t = time / attackDuration;
                boss.GetRigidbody().MovePosition(Vector3.Lerp(startPos, endPos, t));

                if (!hasDealtDamage)
                {
                    Collider2D hit = Physics2D.OverlapCircle((Vector2)boss.transform.position, 0.3f, LayerMask.GetMask("Player", "PlayerDash"));


                    if (hit != null && hit.CompareTag("Player"))
                    {
                        PlayerScript.Instance.TakeDamage(boss);
                        hasDealtDamage = true;
                    }

                }

                time += Time.fixedDeltaTime;
             
                yield return new WaitForFixedUpdate();
            }
            boss.gameObject.layer = LayerMask.NameToLayer("Enemy");
            boss.enemyShaderController.OffOutline();
            yield return new WaitForSeconds(attackPostDelay);
        }


    }
}