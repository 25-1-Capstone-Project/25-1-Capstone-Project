using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
    public int level;
    public abstract void OnEquip(PlayerScript player);
    public abstract void OnUnequip(PlayerScript player);
    // public abstract void AbilityAction
}
