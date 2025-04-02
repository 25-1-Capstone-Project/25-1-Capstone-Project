using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "Player/PlayerStatus")]
public class PlayerStatus : ScriptableObject
{
    [Header("기본 능력")]
    public float speed;
    public int maxHealth;
    public int damage;
    public float attackCooldownSec;
    [Header("투사체")]
    public float bulletSpeed;
  
    [Header("패리 관련")]
    public float parryCooldownSec;
    public float parryDurationSec;
    public int parryStackMax;
    public int currentParryStack;


}
