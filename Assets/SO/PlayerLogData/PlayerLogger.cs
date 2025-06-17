using UnityEngine;

public class PlayerLogger : MonoBehaviour
{
    public static PlayerLogger Instance { get; private set; }
    private PlayerLogData log = new PlayerLogData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetStageComplete(int stage)
    {
        log.max_stage = Mathf.Max(log.max_stage, stage);
    }

    public void PlusEnemyKilledLog() //완
    {
        log.kill_count++;
    }

    public void PlusAttackCountLog() // 완
    {
        log.attack_count++;
    }

    public void AddDamageDealtLog(float damage) // 완
    {
        log.damage_dealt += damage;
    }

    public void AddDamageTakenLog(float damage) // 완
    {
        log.damage_taken += damage;
    }

    public void PlusDeathLog() // 완
    {
        log.death_count++;
    }

    public void PlusSkillUsedLog() // 완 
    {
        log.skill_usage_freq++;
    }

    public void PlusDashLog() // 완
    {
        log.dash_count++;
    }

    public void PlusHitLog() // 완
    {
        log.hit_count++;

    }

    public void AddHealingLog(float amount) // 완
    {
        log.healing_amount += amount;
    }

    public void AddPlaytimeLog(float deltaTime) // 완
    {
        log.playtime += deltaTime;
    }

    public PlayerLogData GetLogData() 
    {
        return log;
    }

}
