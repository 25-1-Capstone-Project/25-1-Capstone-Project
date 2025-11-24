using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_Multiattack")]
public class Enemy_Multiattack : EnemyAttackPattern
{
    int attackIndex = 0;
    public EnemyAttackPattern[] EnemyAttackPatterns;
    void OnValidate()
    {
        attackRange = EnemyAttackPatterns[0].attackRange;
    }
  
    public override IEnumerator Execute(EnemyBase enemy)
    {
        enemy.InitStamina();
        attackIndex = 0;
        enemy.enemyShaderController.OnOutline();
        //  enemy.GetAnimatorController().PlayIdle();
        yield return new WaitForSeconds(attackChargeSec);
        for (int i = 0; i < attackCount; i++)
        {
            enemy.SpriteFlip();
            yield return enemy.StartCoroutine(EnemyAttackPatterns[attackIndex++].Execute(enemy));
        }

    }


}
