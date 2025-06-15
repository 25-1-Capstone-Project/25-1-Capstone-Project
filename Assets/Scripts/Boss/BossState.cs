using System.Collections;
using UnityEngine;

/*
 * =====================================================================================
 *  이 파일은 Boss 클래스 전용 상태들을 정의합니다.
 *  EnemyBase를 상속받는 Boss 클래스와 함께 사용됩니다.
 * =====================================================================================
 */

/// <summary>
/// 모든 보스 상태의 기반이 되는 추상 클래스.
/// 생성자에서 Boss 타입을 받습니다.
/// </summary>
public abstract class BossState : IEnemyState
{
    protected Boss boss;

    public BossState(Boss boss)
    {
        this.boss = boss;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

// -------------------------------------------------------------------------------------

/// <summary>
/// 보스가 전투 시작 시 진입하는 초기 상태. 즉시 Move 상태로 전환됩니다.
/// </summary>
public class BossIdle : BossState
{
    public BossIdle(Boss boss) : base(boss) { }

    public override void Enter()
    {
        // 전투 시작 시 바로 이동 상태로 전환
        boss.StateMachine.ChangeState<BossMove>();
    }
}

// -------------------------------------------------------------------------------------

/// <summary>
/// 플레이어를 추적하고, 공격 범위에 들어오면 공격을 결정하는 상태.
/// </summary>
public class BossMove : BossState, IFixedUpdateState
{
    public BossMove(Boss boss) : base(boss) { }

    public override void Enter()
    {
        boss.GetAnimatorController().PlayChase();
    }

    public override void Update()
    {
        boss.UpdateSkillCooldown();
        
    }

    public void FixedUpdate()
    {
        // 플레이어를 향해 이동
        Vector2 direction = boss.GetDirectionToPlayerNormalVec();
        boss.GetRigidbody().linearVelocity = direction * boss.GetSpeed();
    }

    public void LateUpdate()
    {
        // 플레이어 방향에 따라 스프라이트 뒤집기
        // (EnemyBase에 SpriteFlip()이 구현되어 있어야 함)
        // boss.SpriteFlip(); 
    }

    public override void Exit()
    {

    }
}

// -------------------------------------------------------------------------------------


/// <summary>
/// 보스의 스킬 공격을 수행하는 상태.
/// </summary>
public class BossSkillAttack : BossState
{
    private Coroutine attackRoutine;

    public BossSkillAttack(Boss boss) : base(boss) { }

    public override void Enter()
    {
        Debug.Log($"BossSkillAttack: {boss.GetAttackPattern().name} called");
        boss.GetRigidbody().linearVelocity = Vector2.zero;

        // 1. 퍼지 로직으로 사용할 스킬 결정
        boss.DecideSkill();
        
        attackRoutine = boss.StartCoroutine(AttackSequence());
    }

    public override void Exit()
    {
        if (attackRoutine != null)
        {
            boss.StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
        boss.IsAttacking = false;
        boss.ClearAttackEffect();
    }

    private IEnumerator AttackSequence()
    {
        // EnemyBase의 공격 실행 메서드 호출
        yield return boss.GetAttackPattern().Execute(boss);

        // 공격이 끝나면, 짧은 대기(Cooldown) 상태로 전환
        boss.StateMachine.ChangeState<BossCooldown>();
    }
}

// -------------------------------------------------------------------------------------

/// <summary>
/// 공격(기본/스킬) 이후 다음 행동 전까지 잠시 대기하는 상태.
/// </summary>
public class BossCooldown : BossState
{
    private float _timer = 0f;

    public BossCooldown(Boss boss) : base(boss) { }

    public override void Enter()
    {
        _timer = 0f;
        boss.GetAnimatorController().PlayIdle();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        // Boss 클래스에 설정된 '공격 후 대기 시간'이 지나면
        if (boss.CheckPostAttackPauseComplete(_timer))
        {
            // 다시 이동 상태로 전환하여 다음 행동을 준비
            boss.StateMachine.ChangeState<BossMove>();
        }
    }

    public override void Exit() { }
}

// -------------------------------------------------------------------------------------

/// <summary>
/// 보스가 사망했을 때의 상태.
/// </summary>
public class BossDead : BossState
{
    private Coroutine coroutine;

    public BossDead(Boss boss) : base(boss) { }

    public override void Enter()
    {
        if (coroutine != null) return;

        boss.GetRigidbody().linearVelocity = Vector2.zero;
        boss.GetRigidbody().simulated = false;

        boss.StopAllCoroutines();
        coroutine = boss.StartCoroutine(DeadRoutine());
    }

    public IEnumerator DeadRoutine()
    {
        boss.GetAnimatorController().PlayDeath();
        // EnemyManager가 있다면 사용, 없다면 주석 처리
        // EnemyManager.Instance.KillEnemy(); 
        yield return new WaitForSeconds(2f); // 보스 소멸 연출 시간
        Object.Destroy(boss.gameObject);
    }
}
