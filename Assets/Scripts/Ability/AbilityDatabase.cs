using UnityEngine;
using System.Collections.Generic;
using System.Linq;


[CreateAssetMenu(menuName = "Player/Ability/AbilityDatabase")]
public class AbilityDatabase : ScriptableObject
{
    public List<AbilityData> allAbilities;

    public List<AbilityData> GetUnlockedAbilities(List<AbilityData> currentAbilities)
    {
        Debug.Log("바바바바(어빌디비)");
        return allAbilities.Where(a =>
            !currentAbilities.Contains(a) &&
            (a.prerequisites == null || a.prerequisites.All(p => currentAbilities.Contains(p)))
        ).ToList();
    }
}