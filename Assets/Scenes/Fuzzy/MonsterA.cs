using NUnit.Framework;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class MonsterA : Monster
{
    private Fuzzy fuzzy = new Fuzzy();
    public enum State
    {
        Idle,
        Move,
        Attack
    }

    public Transform player;

    public float moveSpeed = 2f;
    public float health = 100f;
    public float fieldOfView = 60f;
    public float sightRange = 4f;
    public float maxsightRange = 10f;

    public Text healthText;
    public Text distanceText;
    public Text actionText;


    private SpriteRenderer sr;

    protected override void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        _fsm = new FSM(new IdleState(this)); // 초기 상태 설정

    }

    // Update is called once per frame
    protected override void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        distanceText.text = $"Distance: {distance:F2}"; // 플레이어와의 거리 표시

        if (IsPlayerInSight())
        {
            sightRange = maxsightRange; // 시야 범위 증가
            SkillAction action = fuzzy.DecideSkillFuzzy(distance, health);
            UseSkill(action);
            actionText.text = $"Action: {action}"; // 현재 행동 표시

            switch (action)
            {
                case SkillAction.Slash:
                    ChangeState(new meleeAttack(this));
                    Debug.Log("[MonsterA] 상태 변경: Attack");
                    break;
                case SkillAction.Dash:
                    ChangeState(new Move(this));
                    Debug.Log("[MonsterA] 상태 변경: Move");
                    break;
                case SkillAction.Shot:
                    ChangeState(new rangedAttack(this));
                    Debug.Log("[MonsterA] 상태 변경: rangedAttack");
                    break;
                case SkillAction.Idle:
                    ChangeState(new IdleState(this));
                    Debug.Log("[MonsterA] 상태 변경: Idle");
                    break;
            }
        }

        base.Update();
    }

    public override void Attack()
    {
        sr.color = Color.red;
        Debug.Log("[MonsterA] 근접 공격");
    }

    public override void Move()
    {
        sr.color = Color.blue;
        Chase();

        Debug.Log("[MonsterA] 이동");

        
    }

    public override void Idle()
    {
        sr.color = Color.gray;
        Debug.Log("[MonsterA] 대기");
    }


    // 플레이어 탐지 함수
    private bool IsPlayerInSight()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= sightRange)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector2.Angle(directionToPlayer, transform.up);
            if (angleToPlayer < fieldOfView / 2f)
            {
                return true;
            }
        }
        return false;
    }

    void Chase()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;
        transform.position += (Vector3)(toPlayer * moveSpeed * Time.deltaTime);
        Debug.Log("Chasing player..."); // 추적 중일 때마다 로그 출력
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

    void UseSkill(SkillAction action)
    {
        Color color = Color.white;
        string actionName = "스킬 대기중";

        switch (action)
        {
            case SkillAction.Slash: color = Color.red; actionName = "근접 공격"; break;
            case SkillAction.Dash: color = Color.magenta; actionName = "돌진"; break;
            case SkillAction.Shot: color = Color.cyan; actionName = "원거리 공격"; break;
            case SkillAction.Idle: color = Color.gray; actionName = "대기"; break;
        }

        sr.color = color;
        Debug.Log($"[행동] 현재 스킬: {actionName}");
    }

}