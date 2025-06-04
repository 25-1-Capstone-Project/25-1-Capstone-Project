

using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/BossData")]
public class BossData : EnemyBaseData
{
    EnemyAttackPattern[] attackPatterns;
    public override void AttackPatternSet(int index = 0)
    {
        attackPattern = attackPatterns[index];
    }



}
