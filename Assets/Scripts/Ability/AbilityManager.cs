using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private PlayerScript player;

    private readonly List<PlayerAbility> equippedAbilities = new List<PlayerAbility>();
    public IReadOnlyList<PlayerAbility> EquippedAbilities => equippedAbilities;

    public bool EquipAbility(PlayerAbility ability)
    {
        if (equippedAbilities.Contains(ability))
        {
            return false;
        }

        equippedAbilities.Add(ability);
        ability.OnEquip(player);
        return true;
    }

    public bool UnequipAbility(PlayerAbility ability)
    {
        if (!equippedAbilities.Contains(ability))
        {
            return false;
        }
        ability.OnUnequip(player);
        equippedAbilities.Remove(ability);
        return true;
    }

    public void UnequipAll()
    {
        foreach (var ability in equippedAbilities)
        {
            ability.OnUnequip(player);
        }
        equippedAbilities.Clear();
    }
}
