

using UnityEngine;


public abstract class EnemyDataBase : ScriptableObject
{
    public string Name;
    public int maxHealth;
    public float moveSpeed;
    public int attackDamage;
    public bool dontStopEnemy = false;
    public int stamina = 1;
    public RuntimeAnimatorController animator;
    public EnemyAttackPattern attackPattern;
    public float sizeMagnification = 1f;


    public abstract void AttackPatternSet(int index = 0);



}
