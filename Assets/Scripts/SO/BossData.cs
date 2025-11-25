

using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Boss/BossData")]
public class BossData : EnemyDataBase
{
    [SerializeField] EnemyAttackPattern[] attackPatterns;
    public RuntimeAnimatorController spawnAnimator;
    
    public int GetPatternLength() => attackPatterns.Length;
    
    public EnemyAttackPattern GetEnemyAttackPatternAtIndex(int index)
    {
        return attackPatterns[index];
    }
    public void SetAttackPattern(EnemyAttackPattern newAttackPattern)
    {
         attackPattern = newAttackPattern;
    }
    
}
