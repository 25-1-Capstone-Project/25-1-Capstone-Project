using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public abstract class EnemyAttackPattern : ScriptableObject
{
    public abstract IEnumerator Execute(Enemy enemy);
}

