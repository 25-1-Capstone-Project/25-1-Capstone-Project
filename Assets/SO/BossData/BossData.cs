

using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/BossData")]
public class BossData : EnemyBaseData
{
    [SerializeField] EnemyAttackPattern[] attackPatterns;
    public override void AttackPatternSet(int index = 0)
    {
        attackPattern = attackPatterns[index];
    }
}
