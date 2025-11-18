using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_Multiattack")]
public class Enemy_Multiattack : EnemyAttackPattern
{
    public EnemyAttackPattern[] EnemyAttackPatterns;

    public override IEnumerator Execute(EnemyBase enemy)
    {
        enemy.InitStamina();
        int attackIndex = 0;
        enemy.enemyShaderController.OnOutline();
      //  enemy.GetAnimatorController().PlayIdle();
        yield return new WaitForSeconds(attackChargeSec);
        for (int i = 0; i < attackCount; i++)
        {
            yield return enemy.StartCoroutine(EnemyAttackPatterns[attackIndex++].Execute(enemy));
        }

    }


}
