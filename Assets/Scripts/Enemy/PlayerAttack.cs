

using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage;

    public int GetDamage() => damage;
    public void SetDamage(int damage) => this.damage = damage;
}