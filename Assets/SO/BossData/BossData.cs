

using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/BossData")]
public class BossData : EnemyBaseData
{ 
    [SerializeField] public float postAttackPauseTime = 1.5f;
    [SerializeField] public float attackCooldown = 3.0f;
    [SerializeField] EnemyAttackPattern[] attackPatterns;
    public override void AttackPatternSet(int index = 0)
    {
        attackPattern = attackPatterns[index];
    }
    public void SetrandomAttackPattern()
    {
        int randomIndex = Random.Range(0, attackPatterns.Length);
        attackPattern = attackPatterns[randomIndex];
    }
}
