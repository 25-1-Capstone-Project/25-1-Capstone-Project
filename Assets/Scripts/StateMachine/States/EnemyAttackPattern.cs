using UnityEngine;
using System.Collections;

public abstract class EnemyAttackPattern : ScriptableObject
{
    public float attackRange;
    public float attackChargeSec;
    public float attackDuration;
    public float attackPostDelay;
    public abstract IEnumerator Execute(Enemy enemy);
    
}

