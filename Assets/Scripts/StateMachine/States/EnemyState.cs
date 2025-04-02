using System.Collections;
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
        enemy.GetRigidbody().linearVelocity = Vector2.zero; // 정지
    }

    public override void Update()
    {
        // 플레이어가 일정 거리 안에 있으면 추격 상태로 전환
        if (enemy.GetDirectionVec().magnitude < 5f)
        {
            enemy.StateMachine.ChangeState<ChaseState>();
        }
    }

    public override void Exit()
    {

    }
}

// 추격 상태
public class ChaseState : EnemyState, IFixedUpdateState, ILateUpdateState
{
    public ChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.GetAnimatorController().PlayChase();
    }

    public override void Update()
    {
        // 플레이어가 가까우면 공격 상태로 전환
        if (enemy.GetDirectionVec().magnitude < 1f)
        {
            enemy.StateMachine.ChangeState<AttackState>();
        }
    }

    public void FixedUpdate()
    {
        Vector2 direction = enemy.GetDirectionNormalVec();
        enemy.GetRigidbody().linearVelocity = direction * enemy.GetSpeed();
    }

    public void LateUpdate()
    {
        enemy.SpriteFlip();
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
   
    private bool isAttacking;

    public AttackState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SpriteFlip();
        isAttacking = true;
        enemy.GetAnimatorController().PlayAttack();
        enemy.StartCoroutine(AttackRoutine());
    }

    public override void Update()
    {
        // 일정 시간이 지나면 다시 추격 상태로 변경
        if (!isAttacking)
        {
            enemy.StateMachine.ChangeState<ChaseState>();
        }
    }
    private IEnumerator AttackRoutine()
    {
        enemy.Attack();
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
    public override void Exit()
    {
        enemy.StopCoroutine(AttackRoutine());
    }
}
public class KnockBackState : EnemyState
{
    WaitForSeconds KnockBackDelaySec = new WaitForSeconds(0.5f);

    public KnockBackState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.StopAllCoroutines();
        enemy.StartCoroutine(KnockBack());
        
    }
    public IEnumerator KnockBack()
    {
        enemy.KnockBack();
        yield return KnockBackDelaySec;
        enemy.StateMachine.ChangeState<ChaseState>();
    }
    public override void Update() {}
    public override void Exit() {}
}
public class DeadState : EnemyState
{
    Coroutine coroutine;
    public DeadState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        if(coroutine!=null)
        return;

        enemy.StopAllCoroutines();
        coroutine = enemy.StartCoroutine(enemy.DeadRoutine());
    }

    public override void Update() {}
    public override void Exit() {}
}