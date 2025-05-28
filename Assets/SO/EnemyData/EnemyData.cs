
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

    public EnemyData()
    {
        switch (eEnemyType)
        {
            case EEnemyType.CloseRange:
                ChoooseCloseAttackType();
                break;
            case EEnemyType.LongRange:
                ChoooseLongAttackType();
                break;
            default:

                break;
        }
    }
    public void ChoooseCloseAttackType()
    {
        switch (closeAttackType)
        {
            case ECloseAttackType.Slash:
               
                break;
            case ECloseAttackType.Sting:
               
                break;

            default:
                Debug.LogError("Invalid close attack type selected.");
                break;
        }
    }
    public void ChoooseLongAttackType()
    {

        switch (longAttackType)
        {
            case ELongAttackType.SingleShot:
                attackPattern = Resources.Load<EnemyAttackPattern>("Enemy/AttackPattern/Enemy/Enemy_SpearAttack");
                break;
            case ELongAttackType.MultiShot:
                attackPattern = Resources.Load<EnemyAttackPattern>("Enemy/AttackPattern/Enemy/Enemy_BowAttack");
                break;
            default:
                Debug.LogError("Invalid long attack type selected.");
                break;
        }
    }
}
