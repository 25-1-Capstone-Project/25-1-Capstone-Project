using UnityEngine;
using System.Collections;

public abstract class EnemyAttackPattern : ScriptableObject
{
    public int damage = 0;
    public float delay = 0;
    public float range = 0;
    public float cooldown = 0;

    public abstract IEnumerator Execute(Enemy enemy);
}

