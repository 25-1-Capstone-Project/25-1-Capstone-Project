

using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/BossData")]
public class BossData : EnemyDataBase
{ 
    [SerializeField] public float postAttackPauseTime = 1.5f;
    [SerializeField] public float attackCooldown = 3.0f;
    [SerializeField] EnemyAttackPattern[] attackPatterns;
  
   
}
