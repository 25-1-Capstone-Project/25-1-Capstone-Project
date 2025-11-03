
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
    public int maxParryStack;


    [Header("무적 관련")]
    public float flashInterval = 0.1f;  // 깜빡임 속도
    public float invincibleDuration = 1f; // 무적시간
    public float fadeAlpha = 0.3f; // 최소 투명도


}
