

using UnityEngine;


public abstract class EnemyBaseData : ScriptableObject
{
    public string monsterName;
    public int maxHealth;
    public int currentHealth;
    public float moveSpeed;
    public int attackDamage;
    public RuntimeAnimatorController animator;
    public EnemyAttackPattern attackPattern;
    public EEnemyType eEnemyType;

    public abstract void AttackPatternSet();
 
    

}
