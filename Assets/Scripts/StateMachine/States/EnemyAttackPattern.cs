using UnityEngine;
using System.Collections;

public abstract class EnemyAttackPattern : ScriptableObject
{
 

    public abstract IEnumerator Execute(Enemy enemy);
}

