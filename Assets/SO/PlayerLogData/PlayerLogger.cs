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

    public void OnStageComplete(int stage)
    {
        log.max_stage = Mathf.Max(log.max_stage, stage);
    }

    public void OnEnemyKilled() //완
    {
        log.kill_count++;
    }

    public void AttackCount() // 완
    {
        log.attack_count++;
    }

    public void AddDamageDealt(float damage) // 완
    {
        log.damage_dealt += damage;
    }

    public void AddDamageTaken(float damage) // 완
    {
        log.damage_taken += damage;
    }

    public void OnDeath() // 완
    {
        log.death_count++;
    }

    public void OnSkillUsed() // 완 
    {
        log.skill_usage_freq++;
    }

    public void OnDash() // 완
    {
        log.dash_count++;
    }

    public void OnHit() // 완
    {
        log.hit_count++;

    }

    public void AddHealing(float amount) // 완
    {
        log.healing_amount += amount;
    }

    public void AddPlaytime(float deltaTime) // 완
    {
        log.playtime += deltaTime;
    }

    public PlayerLogData GetLogData() 
    {
        return log;
    }

}
