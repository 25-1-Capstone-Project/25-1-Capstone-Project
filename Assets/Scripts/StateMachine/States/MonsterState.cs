using UnityEngine;

// 몬스터 상태 기본 클래스
public abstract class EnemyState : IState
{
    protected Enemy enemy;

    public EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

// Idle 상태
public class IdleState : EnemyState
{
    public IdleState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log("몬스터 대기 중");
        enemy.GetRigidbody().linearVelocity = Vector2.zero; // 정지
    }

    public override void Update()
    {
        // 플레이어가 일정 거리 안에 있으면 추격 상태로 전환
        if (Vector3.Distance(enemy.transform.position, enemy.GetPlayerTransform().position) < 10f)
        {
            enemy.StateMachine.ChangeState<MoveState>();
        }
    }

    public override void Exit()
    {
        Debug.Log("몬스터 이동 시작");
    }
}

// 추격 상태
public class MoveState : EnemyState, IFixedUpdateState
{
    public MoveState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log("몬스터 추격 시작");
    }

    public override void Update()
    {
        // 플레이어가 가까우면 공격 상태로 전환
        if (Vector3.Distance(enemy.transform.position, enemy.GetPlayerTransform().position) < 2f)
        {
            enemy.StateMachine.ChangeState<AttackState>();
        }
    }

    public void FixedUpdate()
    {
        Transform playerTransform = enemy.GetPlayerTransform();
        if (playerTransform != null)
        {
            // 플레이어 방향으로 이동
            Vector2 direction = (playerTransform.position - enemy.transform.position).normalized;
            enemy.GetRigidbody().linearVelocity = direction * enemy.GetSpeed(); // Rigidbody2D의 velocity 사용
        }
    }

    public override void Exit()
    {
        enemy.GetRigidbody().linearVelocity = Vector2.zero; // 추격 종료 시 정지
        Debug.Log("몬스터 추격 종료");
    }
}

// 공격 상태
public class AttackState : EnemyState
{
    private float attackCooldown = 1.5f; // 공격 딜레이
    private float lastAttackTime = 0f;

    public AttackState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log("몬스터 공격!");
        lastAttackTime = Time.time;
    }

    public override void Update()
    {
        // 일정 시간이 지나면 다시 추격 상태로 변경
        if (Time.time - lastAttackTime > attackCooldown)
        {
            enemy.StateMachine.ChangeState<MoveState>();
        }
    }

    public override void Exit()
    {
        Debug.Log("몬스터 공격 종료");
    }
}