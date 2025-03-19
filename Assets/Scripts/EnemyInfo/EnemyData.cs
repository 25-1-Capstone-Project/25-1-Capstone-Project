using UnityEngine;

[CreateAssetMenu(fileName = "Enemy1", menuName = "Scriptable Objects/Enemy1")]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public float maxHealth;
    public float attackDamage;
}
