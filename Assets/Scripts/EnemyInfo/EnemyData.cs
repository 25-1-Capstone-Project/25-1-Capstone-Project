
using System.Collections;
using UnityEngine;

public enum EEnemyType{
    NUll,
    Sowrd,
    Spear,
    Max
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    
    public EEnemyType eEnemyType;
    public string monsterName;
    public int maxHealth;
    public float moveSpeed;
    public float attackSpeed;
    public float attackDamage;


 
}
