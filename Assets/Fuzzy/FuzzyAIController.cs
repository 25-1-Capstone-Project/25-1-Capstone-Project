using UnityEngine;
using UnityEngine.UI;

public enum SkillAction { Idle, Slash, Dash, Shot, Heal }

public class FuzzyAIController : MonoBehaviour
{
    public Transform player;

    [Header("Stats")]
    public float moveSpeed = 2f;
    public float health = 100f;
    public float fieldOfView = 90f;
    public float sightRange = 10f;

    [Header("UI References")]
    public Text healthText;
    public Text distanceText;
    public Text actionText;

    private SkillAction currentSkill;
    private SpriteRenderer sr;
    private bool hasSeenPlayer = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Update UI
        UpdateUI(distance);

        if (IsPlayerInSight())
        {
            hasSeenPlayer = true;
            fieldOfView = 360f;
        }

        // Debug: 체력 조정
        if (Input.GetKeyDown(KeyCode.UpArrow)) health = Mathf.Clamp(health + 10f, 0f, 100f);
        if (Input.GetKeyDown(KeyCode.DownArrow)) health = Mathf.Clamp(health - 10f, 0f, 100f);

        

        if (!hasSeenPlayer)
        {
            currentSkill = SkillAction.Idle;
        }

        else
        {
            currentSkill = DecideSkillFuzzy(distance, health);
            HandleMovement();
        }

        UseSkill(currentSkill);
    }

    void UpdateUI(float distance)
    {
        if (healthText != null) healthText.text = $"체력: {health:F0}";
        if (distanceText != null) distanceText.text = $"거리: {distance:F2}";
        if (actionText != null) actionText.text = $"스킬: {currentSkill}";
    }

    void HandleMovement()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;
        transform.position += (Vector3)(toPlayer * moveSpeed * Time.deltaTime);
    }

    bool IsPlayerInSight()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= sightRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(directionToPlayer, transform.up);
            if (angleToPlayer < fieldOfView / 2f)
            {
                return true;
            }
        }
        return false;
    }
    
//가우시안 함수 계산
    float Gaussian(float x, float c, float sigma)
    {
        return Mathf.Exp(-Mathf.Pow(x - c, 2f) / (2f * Mathf.Pow(sigma, 2f)));
    }

    SkillAction DecideSkillFuzzy(float distance, float hp)
    {
        float near = Gaussian(distance, 1f, 1f);
        float mid = Gaussian(distance, 4f, 1f);
        float far = Gaussian(distance, 8f, 1.5f);

        float low = Gaussian(hp, 20f, 10f);
        float medium = Gaussian(hp, 50f, 10f);
        float high = Gaussian(hp, 80f, 10f);

        Debug.Log($"[퍼지] near: {near:F2}, mid: {mid:F2}, far: {far:F2} | low: {low:F2}, medium: {medium:F2}, high: {high:F2}");

        float slashRule = Mathf.Min(near, high) * 4f;
        float dashRule = Mathf.Min(mid, medium) * 3f;
        float rangedRule = Mathf.Min(far, high) * 2f;
        float healRule = Mathf.Min(far, low) * 1f;

        float totalWeight = slashRule + dashRule + rangedRule + healRule + 0.0001f;
        
        float weightedAverage = (
            slashRule * 5f +
            dashRule * 4f +
            rangedRule * 3f +
            healRule * 2f
        ) / totalWeight;

        Debug.Log($"[퍼지] Weighted Avg: {weightedAverage:F2}");

        if (weightedAverage >= 4.5f) return SkillAction.Slash;
        if (weightedAverage >= 3.5f) return SkillAction.Dash;
        if (weightedAverage >= 2.5f) return SkillAction.Shot;
        if (weightedAverage >= 1.5f) return SkillAction.Heal;
        return SkillAction.Idle;
    }

    void UseSkill(SkillAction action)
    {
        Color color = Color.white;
        string actionName = "스킬 대기중";

        switch (action)
        {
            case SkillAction.Slash: color = Color.red; actionName = "근접 공격"; break;
            case SkillAction.Dash: color = Color.magenta; actionName = "돌진"; break;
            case SkillAction.Shot: color = Color.cyan; actionName = "원거리 공격"; break;
            case SkillAction.Heal: color = Color.green; actionName = "회복"; break;
            case SkillAction.Idle: color = Color.gray; actionName = "대기"; break;
        }

        sr.color = color;
        Debug.Log($"[행동] 현재 스킬: {actionName}");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        float angleStep = 5f;
        for (float angle = -fieldOfView / 2f; angle <= fieldOfView / 2f; angle += angleStep)
        {
            Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.up;
            Gizmos.DrawLine(transform.position, transform.position + direction * sightRange);
        }
    }
}
