using UnityEngine;


public class nonFuzzyAIController : MonoBehaviour
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

        currentSkill = DecideSkill(distance, health);

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

    // 단순 알고리즘으로 스킬 결정
    SkillAction DecideSkill(float distance, float hp)
    {
        // 거리와 체력에 따라 스킬을 결정하는 조건
        if (hp < 30f)
        {
            // 체력이 낮을 때는 회복을 시도
            return SkillAction.Heal;
        }

        if (distance <= 2f)
        {
            // 플레이어가 가까이 있을 때 근접 공격
            return SkillAction.Slash;
        }
        else if (distance <= 5f)
        {
            // 플레이어가 중간 거리일 때 돌진
            return SkillAction.Dash;
        }
        else if (distance <= 8f)
        {
            // 플레이어가 멀리 있을 때 원거리 공격
            return SkillAction.Shot;
        }
        else
        {
            // 아무것도 하지 않을 때 대기
            return SkillAction.Idle;
        }
    }

    void UseSkill(SkillAction action)
    {
        Color color = Color.white;
        string actionName = "스킬 대기중";

        switch (action)
        {
            case SkillAction.Slash: color = Color.red; actionName = "근접 공격(Slash)"; break;
            case SkillAction.Dash: color = Color.magenta; actionName = "돌진(Dash)"; break;
            case SkillAction.Shot: color = Color.cyan; actionName = "원거리 공격(Ranged Shot)"; break;
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
