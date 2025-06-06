using System.Collections;
using UnityEngine;

public abstract class BossState : IEnemyState
{
    protected Boss boss;

    public BossState(Boss boss)
    {
        this.boss = boss;
    }

    public virtual void Enter() { } //상태 진입 시 호출
    public virtual void Update() { } //매 프레임 마다 호출
    public virtual void Exit() { } // 상태가 변경되면 호출
}

public class BossIdle : BossState
{
    float _timer = 0f;
    public BossIdle(Boss boss) : base(boss) { }

    public override void Enter()
    {
        boss.StateMachine.ChangeState<BossMove>(); // Idle 상태에서 바로 이동 상태로 변경
        // boss.GetRigidbody().linearVelocity = Vector2.zero; // 정지
        // boss.GetAnimatorController().PlayIdle();
    }

    public override void Update()
    {
        // _timer += Time.deltaTime;

        // if (boss.CheckCooldownComplete(_timer))
        // {
        //     boss.StateMachine.ChangeState<BossMove>(); // 쿨타임이 끝나면 이동 상태로 변경
        //     _timer = 0f; // 타이머 초기화
        // }

    }

    public override void Exit()
    {

    }
}

public class BossAttack : BossState
{
    private Coroutine attackRoutine;

    public BossAttack(Boss boss) : base(boss) { }

    public override void Enter()
    {
        boss.GetRigidbody().linearVelocity = Vector2.zero;
        boss.DecideSkill();
        boss.Attack();
        attackRoutine = boss.StartCoroutine(AttackSequence());
    }


    public override void Update() { }

    public override void Exit()
    {
        if (attackRoutine != null)
        {
            boss.StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        boss.IsAttacking = false;
        boss.ClearAttackEffect(); // 예고선 정리
    }

    private IEnumerator AttackSequence()
    {
        yield return boss.GetAttackPattern().Execute(boss);

        if (boss.CheckAttackRange())
            boss.StateMachine.ChangeState<BossAttack>();
        else
            boss.StateMachine.ChangeState<BossMove>();
    }

}

public class BossCooldown : BossState
{

    private float _timer = 0f;
    public BossCooldown(Boss monster) : base(monster) { }

    public override void Enter()
    {
        _timer = 0f;
        boss.GetAnimatorController().PlayIdle();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (boss.CheckCooldownComplete(_timer))
        {
            boss.StateMachine.ChangeState<BossMove>(); // 쿨타임이 끝나면 이동 상태로 변경
        }

    }

    public override void Exit()
    {
    }
}

public class BossMove : BossState, IFixedUpdateState
{
    public BossMove(Boss monster) : base(monster) { }

    float _timer = 0f;
    public override void Enter()
    {
        _timer = 0f;
        boss.GetAnimatorController().PlayChase();
    }

    public override void Update()
    {
        // _timer += Time.deltaTime;
        // if (_timer > Random.Range(2f, 4f))
        // {
        //     boss.StateMachine.ChangeState<BossAttack>(); // 일정 시간 후 준비 상태로 전환
        //     _timer = 0f; // 타이머 초기화
        // }
        if (boss.GetDirectionToPlayerVec().magnitude < 5f)
        {
            boss.StateMachine.ChangeState<BossAttack>();
        }
    }

    public void FixedUpdate()
    {
        Vector2 direction = boss.GetDirectionToPlayerNormalVec();
        boss.GetRigidbody().linearVelocity = direction * boss.GetSpeed();
    }

    public void LateUpdate()
    {
        boss.SpriteFlip();
    }


    public override void Exit()
    {
        boss.GetRigidbody().linearVelocity = Vector2.zero; // 추격 종료 시 정지
    }
}

public class BossDead : BossState
{
    public BossDead(Boss boss) : base(boss) { }

    Coroutine coroutine;
    public override void Enter()
    {
        if (coroutine != null)
            return;
        boss.GetRigidbody().linearVelocity = Vector2.zero;
        boss.GetRigidbody().simulated = false; // 상호작용 비활성화

        boss.StopAllCoroutines();

        coroutine = boss.StartCoroutine(DeadRoutine());
    }
    public IEnumerator DeadRoutine()
    {
        boss.GetAnimatorController().PlayDeath();
        EnemyManager.Instance.KillEnemy();
        yield return new WaitForSeconds(1f);
        Object.Destroy(boss.gameObject);
    }
    public override void Update() { }
    public override void Exit() { }


}
