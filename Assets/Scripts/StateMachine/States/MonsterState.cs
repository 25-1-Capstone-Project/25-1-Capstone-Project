using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

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
        
        enemy.GetRigidbody().linearVelocity = Vector2.zero; // 정지
    }

    public override void Update()
    {
        // 플레이어가 일정 거리 안에 있으면 추격 상태로 전환
        if (Vector3.Distance(enemy.transform.position, GameManager.instance.GetPlayerTransform().position) < 10f)
        {
            enemy.StateMachine.ChangeState<ChaseState>();
        }
    }

    public override void Exit()
    {

    }
}

// 추격 상태
public class ChaseState : EnemyState, IFixedUpdateState
{
    public ChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.GetAnimatorController().PlayChase();
    }

    public override void Update()
    {
        // 플레이어가 가까우면 공격 상태로 전환
        if (Vector3.Distance(enemy.transform.position, GameManager.instance.GetPlayerTransform().position) < 1f)
        {
            enemy.StateMachine.ChangeState<AttackState>();
        }
    }

    public void FixedUpdate()
    {
        Transform playerTransform = GameManager.instance.GetPlayerTransform();
        if (playerTransform != null)
        {
            // 플레이어 방향으로 이동
            Vector2 direction = (playerTransform.position - enemy.transform.position).normalized;
            enemy.GetRigidbody().linearVelocity = direction * enemy.GetSpeed();
            //enemy.transform.rotation = Quaternion.LookRotation(direction); // 스프라이트로 대체해야함
        }
    }

    public override void Exit()
    {
        enemy.GetRigidbody().linearVelocity = Vector2.zero; // 추격 종료 시 정지
    }
}

// 공격 상태
public class AttackState : EnemyState
{
    private float attackCooldown = 1.5f; //공격 시간
    private float beforAttackCooldown = 1.5f; //선딜
    private bool isAttacking;

    public AttackState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.GetAnimatorController().PlayAttack();
        isAttacking = true;
        enemy.StartCoroutine(Attack());

    }

    public override void Update()
    {
        // 일정 시간이 지나면 다시 추격 상태로 변경
        if (!isAttacking)
        {
            enemy.StateMachine.ChangeState<ChaseState>();
        }

    }
    private IEnumerator Attack()
    {
        // 여기서 애니메이션 재생 또는 실제 공격 로직 실행 가능
        Debug.Log("Attack started");

        // 예: 공격 애니메이션 재생 시간
        yield return new WaitForSeconds(attackCooldown);

        Debug.Log("Attack finished");
        isAttacking = false;
    }
    public override void Exit()
    {
        enemy.StopCoroutine(Attack());
    }
}