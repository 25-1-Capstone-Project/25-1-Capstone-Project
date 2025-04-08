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

    }

    public override void Update()
    {
        enemy.GetAnimatorController().PlayChase();
        // 플레이어가 가까우면 공격 상태로 전환
        if (enemy.GetDirectionVec().magnitude < 1f)
        {
            enemy.StateMachine.ChangeState<AttackState>();
        }
    }

    public void FixedUpdate()
    {
        if (enemy.GetDirectionVec().magnitude < 1f)
            return;

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
    public AttackState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
            enemy.Attack();
            enemy.StartCoroutine(WaitAttackCooldown());
    }

    private IEnumerator WaitAttackCooldown()
    {
        yield return new WaitForSeconds(enemy.GetAttackPattern().cooldown); // 정확히 기다리고
        if (enemy.GetDirectionVec().magnitude < 1f)
        {
            enemy.StateMachine.ChangeState<AttackState>();
        }
        else
        {
            enemy.StateMachine.ChangeState<ChaseState>();
        }

    }

    public override void Update() { }
    public override void Exit() { }
}
public class ParryState : EnemyState
{
    WaitForSeconds KnockBackDelaySec = new WaitForSeconds(0.5f);

    public ParryState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.StartCoroutine(KnockBack());
    }
    public IEnumerator KnockBack()
    {
        enemy.KnockBack(2);
        yield return KnockBackDelaySec;
        enemy.StateMachine.ChangeState<ChaseState>();
    }
    public override void Update() { }
    public override void Exit() { }
}
//패링 당했을때


public class DamagedState : EnemyState
{
    public DamagedState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.StopAllCoroutines();
        // enemy.GetAnimatorController().PlayDamaged();
        enemy.StartCoroutine(DamagedRoutine());
    }
    public IEnumerator DamagedRoutine()
    {
        enemy.KnockBack(1);
        enemy.FlashOnDamage();
        yield return new WaitForSeconds(0.5f);

        enemy.StateMachine.ChangeState<ChaseState>();
    }

    public override void Update() { }
    public override void Exit() { }
}

public class DeadState : EnemyState
{
    Coroutine coroutine;
    public DeadState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        if (coroutine != null)
            return;
        enemy.GetRigidbody().linearVelocity = Vector2.zero;
        enemy.GetRigidbody().simulated = false; // 상호작용 비활성화

        enemy.StopAllCoroutines();

        coroutine = enemy.StartCoroutine(DeadRoutine());
    }
    public IEnumerator DeadRoutine()
    {
        enemy.GetAnimatorController().PlayDeath();
        yield return new WaitForSeconds(1f);
        Object.Destroy(enemy.gameObject);
    }
    public override void Update() { }
    public override void Exit() { }
}