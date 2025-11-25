using UnityEngine;
using System.Collections;


[CreateAssetMenu(menuName = "Boss/AttackPattern/Wood/Boss_SwordAttack")]
public class Boss_SwordAttack : EnemyAttackPattern
{

    public float attackSpeed;

    public override IEnumerator Execute(EnemyBase enemy)
    {
        enemy.GetRigidbody().linearVelocity = Vector2.zero;

        enemy.GetAnimatorController().PlayAttack();
        enemy.enemyShaderController.OnOutline();
        //    yield return enemy.StartCoroutine(enemy.OutLineRoutine(attackChargeSec));
        yield return new WaitForSeconds(attackChargeSec);

        enemy.AddAttackEffect(EffectPooler.Instance.SpawnFromPool("WoodProjectile", enemy.transform.position + Vector3.up*2));
        yield return null;
        enemy.AddAttackEffect(EffectPooler.Instance.SpawnFromPool("WoodProjectile", enemy.transform.position + new Vector3(2f, 1f, 0)));
        yield return null;
        enemy.AddAttackEffect(EffectPooler.Instance.SpawnFromPool("WoodProjectile", enemy.transform.position + new Vector3(-2f, 1f, 0)));
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(0.3f);
        enemy.enemyShaderController.OffOutline();
        yield return new WaitForSeconds(attackPostDelay);
    }
}