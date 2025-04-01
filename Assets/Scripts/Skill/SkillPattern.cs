using UnityEngine;
using System.Collections;

public abstract class SkillPattern: ScriptableObject
{
    public abstract IEnumerator Act(Player player);
}
