

using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/BossData")]
public class BossData : ScriptableObject
{

    public string monsterName;
    public int maxHealth;
    public int currentHealth;
    public float moveSpeed;
    public float attackSpeed;
    public float attackRange;
    public int attackDamage;
    public RuntimeAnimatorController animator;
    public EEnemyType eEnemyType;
    public EnemyAttackPattern[] attackPattern;


    public void AttackPatternSet()
    {
        // 공격 패턴 설정 로직
    }



}
