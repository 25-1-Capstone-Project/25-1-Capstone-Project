using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "Scriptable Objects/PlayerStatus")]
public class PlayerStatus : ScriptableObject
{
    [Header("기본 능력")]
    public float speed;
    public float health;

    [Header("투사체")]
    public float bulletSpeed;
  
    [Header("패리 관련")]
    public float parryCooldown;
    public float parryDuration;
    public int parryStackMax;
    public int currentParryStack;


}
