using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_SingleGunAttack")]
public class Enemy_SingleGunAttack : EnemyAttackPattern
{
    public float effectWidth = 0.4f;

    public override IEnumerator Execute(Enemy enemy)
    {
        yield return new WaitForSeconds(attackChargeSec);
        GameObject attackProjectile = EffectPooler.Instance.SpawnFromPool("EnemyAttackProjectile1", enemy.transform.position, Quaternion.identity);
        attackProjectile.tag = "EnemyAttack";
        EnemyAttack enemyAttack = attackProjectile.GetComponent<EnemyAttack>();
        enemyAttack.SetDamage(enemy.GetDamage());
        enemyAttack.SetDirectionVec(enemy.GetDirectionToPlayerNormalVec());
        enemy.SpriteFlip();
        yield return new WaitForSeconds(attackPostDelay);


    }
}
