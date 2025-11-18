

using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage;
    public bool disableOnEnemyHit;

    public int GetDamage() => damage;
    public void SetDamage(int damage) => this.damage = damage;
}