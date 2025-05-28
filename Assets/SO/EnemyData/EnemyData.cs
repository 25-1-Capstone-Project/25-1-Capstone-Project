
using JetBrains.Annotations;
using Mono.Cecil.Cil;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{

    public string monsterName;
    public int maxHealth;
    public float moveSpeed;
    public float attackSpeed;
    public float attackRange;
    public int attackDamage;
    public RuntimeAnimatorController animator;
    public EnemyAttackPattern attackPattern;
    public ECloseAttackType closeAttackType;
    public ELongAttackType longAttackType;
    public EEnemyType eEnemyType;

    public void AttackPatternSet()
    {
        switch (eEnemyType)
        {
            case EEnemyType.CloseRange:
                attackPattern = EnemyManager.Instance.CloseEnemyAttackPatterns[(int)closeAttackType];

                break;
            case EEnemyType.LongRange:
                attackPattern = EnemyManager.Instance.LongenemyAttackPatterns[(int)longAttackType];

                break;
        }
    }
  
    

}
