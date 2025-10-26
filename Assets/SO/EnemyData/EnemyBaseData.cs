

using UnityEngine;


public abstract class EnemyDataBase : ScriptableObject
{
    public string Name;
    public int maxHealth;
    public int currentHealth;
    public float moveSpeed;
    public int attackDamage;
    public bool dontStopEnemy = false;
    public RuntimeAnimatorController animator;
    public EnemyAttackPattern attackPattern;
    public float sizeMagnification = 1f;


    public abstract void AttackPatternSet(int index = 0);



}
