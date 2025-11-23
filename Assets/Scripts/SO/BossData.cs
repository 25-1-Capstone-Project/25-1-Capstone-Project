

using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Boss/BossData")]
public class BossData : EnemyDataBase
{
    [SerializeField] EnemyAttackPattern[] attackPatterns;
    public RuntimeAnimatorController spawnAnimator;

}
