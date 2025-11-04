using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_HumanWarrior")]
public class Enemy_HumanWarrior : EnemyAttackPattern
{
    public EnemyAttackPattern[] EnemyAttackPatterns;
    public override IEnumerator Execute(EnemyBase enemy)
    {
        enemy.InitStamina();
        int attackIndex = 0;
        enemy.enemyShaderController.OnOutline();
        enemy.GetAnimatorController().PlayIdle();
         yield return new WaitForSeconds(attackChargeSec);

        yield return enemy.StartCoroutine(EnemyAttackPatterns[attackIndex++].Execute(enemy));

        yield return enemy.StartCoroutine(EnemyAttackPatterns[attackIndex++].Execute(enemy));

        yield return enemy.StartCoroutine(EnemyAttackPatterns[attackIndex].Execute(enemy));


    }


}
