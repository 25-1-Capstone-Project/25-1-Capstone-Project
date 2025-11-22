/// <summary>
/// 런타임 데이타 복사용 클래스
/// </summary>
[System.Serializable]
public class PlayerRuntimeStats
{
    public float speed;
    public int maxHealth;
    public int currentHealth;
    public int damage;
    public float attackCooldownSec;
 
    public float attackAngle;

    public float parryCooldownSec;
    public float parryDurationSec;
    public int maxParryStack;
    public int currentParryStack;

    public void ApplyBase(PlayerData baseData)
    {
        speed = baseData.speed;
        maxHealth = baseData.maxHealth;
        currentHealth = maxHealth;

        parryCooldownSec = baseData.parryCooldownSec;
        parryDurationSec = baseData.parryDurationSec;
   
    }
}