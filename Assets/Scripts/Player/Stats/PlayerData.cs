using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "Player/PlayerStatus")]
public class PlayerData : ScriptableObject
{
    [Header("기본 능력")]
    public float speed;
    public int maxHealth;
    public int damage;
    public float attackCooldownSec;
    public float attackRange;
    public float attackAngle = 120;

  
    [Header("패리 관련")]
    public float parryCooldownSec;
    public float parryDurationSec;
    public int currentParryStack;
    public int maxParryStack ;


}
