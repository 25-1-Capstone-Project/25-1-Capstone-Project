

using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : EnemyDataBase
{
    public EEnemyType eEnemyType;
    public ECloseAttackType closeAttackType;
    public ELongAttackType longAttackType;
    public ESpecialAttackType specialAttackType;

    
    public override void AttackPatternSet(int index)
    {
        switch (eEnemyType)
        {
            case EEnemyType.CloseRange:
                attackPattern = EnemyManager.Instance.CloseEnemyAttackPatterns[(int)closeAttackType];
                break;
            case EEnemyType.LongRange:
                attackPattern = EnemyManager.Instance.LongenemyAttackPatterns[(int)longAttackType];
                break;
            case EEnemyType.SpecialAttack:
                attackPattern = EnemyManager.Instance.SpecialEnemyAttackPatterns[(int)specialAttackType];
                break;
        }
    }



}
