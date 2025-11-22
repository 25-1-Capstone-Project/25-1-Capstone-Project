using System.Collections;
using UnityEngine;


// 몬스터 상태 기본 클래스
public abstract class EnemyState : IState
{
    protected EnemyBase enemy;

    public EnemyState(EnemyBase enemy)
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
    public IdleState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.GetRigidbody().linearVelocity = Vector2.zero; // 정지
    }

    public override void Update()
    {
        // 플레이어가 일정 거리 안에 있으면 추격 상태로 전환
        if (enemy.GetDirectionToPlayerVec().magnitude < 5f)
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
    public ChaseState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {

        enemy.GetAnimatorController().PlayChase();
    }

    public override void Update()
    {
        if (enemy.CheckAttackRange())
            enemy.StateMachine.ChangeState<AttackState>(); // 공격 범위 체크

    }

    public void FixedUpdate()
    {
        if (enemy.CheckAttackRange())
            return;

        enemy.GetAIAgent().Move();
        // Vector2 direction = enemy.GetDirectionToPlayerNormalVec();
        // enemy.GetRigidbody().linearVelocity = direction * enemy.GetSpeed();
    }


    public void LateUpdate()
    {
        enemy.SpriteFlip(); // 플레이어 방향으로 스프라이트 회전
    }

    public override void Exit()
    {
        enemy.GetAIAgent().Stop();
        enemy.GetRigidbody().linearVelocity = Vector2.zero; // 추격 종료 시 정지
    }
}

// 공격 상태
public class AttackState : EnemyState
{
    private Coroutine attackRoutine;

    public AttackState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.GetRigidbody().linearVelocity = Vector2.zero;
        enemy.IsAttacking = true;
        attackRoutine = enemy.StartCoroutine(AttackSequence());
    }
    public override void Update() { }

    public override void Exit()
    {
        if (attackRoutine != null)
        {
            enemy.StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        enemy.IsAttacking = false;
        enemy.ClearAttackEffect(); // 예고선 정리
    }

    private IEnumerator AttackSequence()
    {
        yield return enemy.GetAttackPattern().Execute(enemy);

        if (enemy.CheckAttackRange())
            enemy.StateMachine.ChangeState<AttackState>();
        else
            enemy.StateMachine.ChangeState<ChaseState>();
    }
}

public class ParriedState : EnemyState
{


    public ParriedState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.StopAllCoroutines();
        enemy.StartCoroutine(ParriedRoutine());
        enemy.enemyShaderController.OnOutline();
        enemy.IsStunned = true;
        enemy.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
    public IEnumerator ParriedRoutine()
    {
        enemy.KnockBack(2);
         enemy.GetAnimatorController().PlayIdle();
        yield return new WaitForSeconds(0.1f);
        enemy.GetAnimatorController().FreezeFrame(true);
        yield return new WaitForSeconds(0.7f);
        enemy.GetRigidbody().linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(2f);
        enemy.GetAnimatorController().FreezeFrame(false);
        enemy.InitStamina();
        enemy.enemyShaderController.OffOutline();

        enemy.StateMachine.ChangeState<ChaseState>();
    }
    public override void Update() { }
    public override void Exit() { enemy.IsStunned = false; }
}

public class DamagedState : EnemyState
{
    public DamagedState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.StartCoroutine(DamagedRoutine());
    }
    public IEnumerator DamagedRoutine()
    {
        enemy.enemyShaderController.OffOutline();
        enemy.GetRigidbody().linearVelocity = Vector2.zero;
        enemy.gameObject.layer = LayerMask.NameToLayer("Enemy");

        yield return new WaitForSeconds(1f);
        enemy.StopAllCoroutines();
        enemy.SetCurrentAnimator();
        enemy.StateMachine.ChangeState<ChaseState>();
    }

    public override void Update() { }
    public override void Exit() { }
}

public class DeadState : EnemyState
{
    Coroutine coroutine;
    public DeadState(EnemyBase enemy) : base(enemy) { }

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

        EnemyManager.Instance.KillEnemy();
        yield return new WaitForSeconds(1f);
        Object.Destroy(enemy.gameObject);
    }
    public override void Update() { }
    public override void Exit() { }
}