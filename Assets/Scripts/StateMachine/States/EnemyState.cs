using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public interface IEnemyState : IState { }

public abstract class EnemyState : IEnemyState
{
    protected readonly EnemyBase enemy;
    protected EnemyState(EnemyBase enemy) { this.enemy = enemy; }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

/* -------------------- Idle -------------------- */
public class IdleState : EnemyState
{
    public IdleState(EnemyBase enemy) : base(enemy) { }
    public override void Enter() => enemy.GetRigidbody().linearVelocity = Vector2.zero;
    public override void Update()
    {
        if (enemy.GetDirectionToPlayerVec().sqrMagnitude < 25f) // 5^2
            enemy.StateMachine.ChangeState<ChaseState>();
    }
    public override void Exit() { }
}

/* -------------------- Chase -------------------- */
public class ChaseState : EnemyState, IFixedUpdateState, ILateUpdateState
{
    public ChaseState(EnemyBase enemy) : base(enemy) { }
    public override void Enter() => enemy.GetAnimatorController()?.PlayChase();

    public override void Update()
    {
        if (enemy.CheckAttackRange())
            enemy.StateMachine.ChangeState<AttackState>();
    }

    public void FixedUpdate()
    {
        if (enemy.CheckAttackRange()) return;
        enemy.Move();
    }

    public void LateUpdate() => enemy.SpriteFlip();

    public override void Exit() => enemy.GetRigidbody().linearVelocity = Vector2.zero;
}

/* -------------------- Attack -------------------- */
public class AttackState : EnemyState
{
    public AttackState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.GetRigidbody().linearVelocity = Vector2.zero;
        enemy.IsAttacking = true;
        enemy.RunAction(AttackSequence()); // 🔧 전용 러너 사용
    }

    public override void Update() { }

    public override void Exit()
    {
        enemy.StopAction();                // 🔧 해당 상태 코루틴만 중지
        enemy.IsAttacking = false;
        enemy.ClearAttackEffect();
    }

    private IEnumerator AttackSequence()
    {
        var pattern = enemy.GetData()?.attackPattern;
        if (pattern != null)
            yield return pattern.Execute(enemy);
            
        if (enemy.CheckAttackRange()) enemy.StateMachine.ChangeState<AttackState>();
        else enemy.StateMachine.ChangeState<ChaseState>();
    }
}

/* -------------------- Parried -------------------- */
public class ParriedState : EnemyState
{
    public ParriedState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.StopAction(); // 🔧 전 상태 코루틴만 안전 중지
        enemy.GetRigidbody().linearVelocity = Vector2.zero;
        enemy.enemyShaderController?.OnOutline();
        enemy.RunAction(ParriedRoutine());
    }

    private IEnumerator ParriedRoutine()
    {
        // 그로기 연출
        yield return new WaitForSecondsRealtime(2f);
        enemy.InitStamina();
        enemy.enemyShaderController?.OffOutline();
        enemy.StateMachine.ChangeState<ChaseState>();
    }

    public override void Update() { }
    public override void Exit() { }
}

/* -------------------- Damaged (with temp layer) -------------------- */
public class DamagedState : EnemyState
{
    public DamagedState(EnemyBase enemy) : base(enemy) { }

    private readonly WaitForSeconds KnockBackHold = new WaitForSeconds(0.5f);
    private int originalLayer;

    public override void Enter()
    {
        enemy.enemyShaderController?.OffOutline();
        enemy.GetRigidbody().linearVelocity = Vector2.zero;

        // 🔧 적-적 충돌 완전 차단을 위해 임시 레이어로 전환 (Physics2D 매트릭스에서 EnemyDamaged↔Enemy OFF)
        originalLayer = enemy.gameObject.layer;
        enemy.gameObject.layer = LayerMask.NameToLayer("EnemyDamaged");

        enemy.GetAnimatorController()?.PlayDamage();
        if (!enemy.GetData().dontStopEnemy)
        {
            enemy.StopAction();
            enemy.RunAction(KnockBackRoutine());
        }
    }

    private IEnumerator KnockBackRoutine(float time = 1f)
    {
        enemy.KnockBack(2f);
        yield return KnockBackHold;
        enemy.GetRigidbody().linearVelocity = Vector2.zero;

        // 🔧 레이어 복구 후 추격 복귀
        yield return new WaitForSeconds(time);
        enemy.gameObject.layer = originalLayer;
        enemy.StateMachine.ChangeState<ChaseState>();
    }

    public override void Update() { }
    public override void Exit()
    {
        // 안전 복구
        enemy.gameObject.layer = originalLayer;
    }
}

/* -------------------- Dead -------------------- */
public class DeadState : EnemyState
{
    public DeadState(EnemyBase enemy) : base(enemy) { }

    private float knockBackDistance = 5f;
    private float time = 0.4f;

    public override void Enter()
    {
        enemy.GetRigidbody().linearVelocity = Vector2.zero;
        enemy.GetRigidbody().simulated = false; // 상호작용 비활성화
        enemy.StopAction();
        enemy.RunAction(DeadRoutine());
    }

    private IEnumerator DeadRoutine()
    {
        Vector2 origin = enemy.transform.position;
        Vector2 target = origin + -enemy.GetDirectionNormalVec() * knockBackDistance;

        // 🔧 다음 위치를 기준으로 벽 충돌 검사 + 실제 위치 갱신
        for (float t = 0f; t < 1f; t += Time.deltaTime / time)
        {
            Vector2 next = Vector2.Lerp(origin, target, t);
            if (Physics2D.OverlapCircle(next, 0.5f, LayerMask.GetMask("Wall")))
                break;

            enemy.transform.position = next;
            yield return null;
        }

        enemy.GetAnimatorController()?.PlayDeath();
        EnemyManager.Instance?.KillEnemy();
        yield return new WaitForSeconds(1f);
        Object.Destroy(enemy.gameObject);
    }

    public override void Update() { }
    public override void Exit() { }
}
