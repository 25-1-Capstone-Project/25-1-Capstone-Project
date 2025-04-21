using UnityEngine;

public enum SkillAction { Idle, Slash, Dash, RangedShot, Heal }

public class FuzzyAIController : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float health = 100f;
    public float fieldOfView = 90f;
    public float sightRange = 10f;

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

        if (IsPlayerInSight())
        {
            hasSeenPlayer = true;
            fieldOfView = 360f;
        }

        // 실시간 체력 조절 (디버그용)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            health = Mathf.Clamp(health + 10f, 0f, 100f);
            Debug.Log("체력 증가: " + health);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            health = Mathf.Clamp(health - 10f, 0f, 100f);
            Debug.Log("체력 감소: " + health);
        }

        currentSkill = DecideSkillFuzzy(distance, health);

        if (hasSeenPlayer)
        {
            HandleMovement();
        }

        UseSkill(currentSkill);
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

    // 퍼지 로직 적용
    // 삼각형 퍼지 소속 함수
    float Triangular(float x, float a, float b, float c)
    {
        if (x <= a || x >= c) return 0f;
        else if (x == b) return 1f;
        else if (x < b) return (x - a) / (b - a);
        else return (c - x) / (c - b);
    }

    SkillAction DecideSkillFuzzy(float distance, float hp)
    {
        float near = Triangular(distance, 0f, 1.5f, 3f);     // 가까움
        float mid = Triangular(distance, 2f, 4f, 6f);       // 중간
        float far = Triangular(distance, 5f, 7.5f, 10f);    // 멀리

        // 체력에 따른 소속 함수
        float low = Triangular(hp, 0f, 25f, 50f);           // 체력 낮음
        float medium = Triangular(hp, 30f, 50f, 70f);       // 체력 중간
        float high = Triangular(hp, 60f, 80f, 100f);        // 체력 높음

        Debug.Log($"[퍼지] near: {near:F2}, mid: {mid:F2}, far: {far:F2} | low: {low:F2}, medium: {medium:F2}, high: {high:F2}");

        // 각 규칙의 값 계산
        float slashRule = Mathf.Min(near, high) * 5f;       // 근접 공격
        float dashRule = Mathf.Min(mid, medium) * 4f;       // 돌진
        float rangedRule = Mathf.Min(far, high) * 3f;       // 원거리 공격
        float healRule = Mathf.Min(near, low) * 2f;         // 회복
        float idleRule = Mathf.Min(far, low) * 1f;          // 대기

        float totalWeight = slashRule + dashRule + rangedRule + healRule + idleRule + 0.0001f;
        float weightedAverage = (
            slashRule * 5f +
            dashRule * 4f +
            rangedRule * 3f +
            healRule * 2f +
            idleRule * 1f
        ) / totalWeight;

        Debug.Log($"[퍼지] Weighted Avg: {weightedAverage:F2}");

        if (weightedAverage >= 4.5f) return SkillAction.Slash;
        if (weightedAverage >= 3.5f) return SkillAction.Dash;
        if (weightedAverage >= 2.5f) return SkillAction.RangedShot;
        if (weightedAverage >= 1.5f) return SkillAction.Heal;
        return SkillAction.Idle;
    }

    void UseSkill(SkillAction action)
    {
        Color color = Color.white;
        string actionName = "스킬 대기중";

        switch (action)
        {
            case SkillAction.Slash: color = Color.red; actionName = "근접 공격(Slash)"; break;
            case SkillAction.Dash: color = Color.magenta; actionName = "돌진(Dash)"; break;
            case SkillAction.RangedShot: color = Color.cyan; actionName = "원거리 공격(Ranged Shot)"; break;
            case SkillAction.Heal: color = Color.green; actionName = "회복(Heal)"; break;
            case SkillAction.Idle: color = Color.gray; actionName = "대기(Idle)"; break;
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
