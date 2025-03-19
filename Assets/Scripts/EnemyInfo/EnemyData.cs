using UnityEngine;

[CreateAssetMenu(fileName = "Enemy1", menuName = "Scriptable Objects/Enemy1")]
public class EnemyData : ScriptableObject
{
    public int id = 0;
    public string monsterName;
    public int maxHealth;
    public float moveSpeed;
    public float attackSpeed;
    public float attackDamage;
   
}
