using System.Collections;
using UnityEngine;

public abstract class BossState : IState
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
    private float _timer = 0f;
    public BossIdle(Boss monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
        _timer = 0f;
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > 2f) // 2초 후에 상태 변경
        {
            boss.StateMachine.ChangeState<BossPreparing>();
        }
        Debug.Log("In Idle State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}
public class BossPreparing : BossState
{
    private float _timer = 0f;
    private float prepareTime = 2f; // 준비 시간
    public bool IsPreparingComplete() => _timer >= prepareTime;
    public BossPreparing(Boss monster) : base(monster) { }

    public override void Enter()
    {
        _timer = 0f;

    }

    public override void Update()
    {
        _timer += Time.deltaTime;

    }

    public override void Exit()
    {

    }
}
public class BossAttack : BossState
{
    private float _timer;
    private float attackTime = 3f; // 공격 시간
    public BossAttack(Boss monster) : base(monster) { }

    public override void Enter()
    {
        boss.DecideSkill();
        boss.Attack();
    }

    public override void Update()
    {
       
    }

    public override void Exit()
    {

    }

    public bool IsAttackFinished()
    {
        return _timer >= attackTime;
    }
}

public class BossCooldown : BossState
{

    private float _timer = 0f;
    public BossCooldown(Boss monster) : base(monster) { }

    public override void Enter()
    {
        _timer = 0f;
        boss.GetAnimatorController().PlayChase();
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

    public override void Enter()
    {
        boss.GetAnimatorController().PlayChase();
    }

    public override void Update()
    {
        //일정 시간이 지나면 공격하게 변경예정
        // if (boss.CheckAttackRange())
        //     boss.StateMachine.ChangeState<AttackState>(); // 공격 범위 체크
    }

    public void FixedUpdate()
    {
        if (boss.CheckAttackRange())
            return;

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
    public BossDead(Boss monster) : base(monster) { }

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
