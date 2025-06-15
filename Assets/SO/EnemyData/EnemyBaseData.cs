

using UnityEngine;


public abstract class EnemyBaseData : ScriptableObject
{
    public string Name;
    public int maxHealth;
    public int currentHealth;
    public float moveSpeed;
    public int attackDamage;
    public RuntimeAnimatorController animator;
    public EnemyAttackPattern attackPattern;
 

    public abstract void AttackPatternSet(int index = 0);
 
    

}
