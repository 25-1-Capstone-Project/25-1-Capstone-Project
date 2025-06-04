using UnityEngine;

public abstract class BossState : IState
{
    protected Boss _boss;

    public BossState(Boss boss)
    {
        _boss = boss;
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
            _boss.StateMachine.ChangeState<BossPreparing>();
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
        _timer = 0f;
      
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
     
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
    private float cooldownTime = 2.0f;
    private float _timer = 0f;
    public bool IsCooldownComplete => _timer >= cooldownTime;
    public BossCooldown(Boss monster) : base(monster) { }

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

public class BossMove : BossState
{
    public BossMove(Boss monster) : base(monster) { }

    public override void Enter()
    {

    }

    public override void Update()
    {
     
    }

    public override void Exit()
    {
       // _boss.MoveToPlayer();
    
    }
}

public class BossDead : BossState
{
    public BossDead(Boss monster) : base(monster) { }

    public override void Enter()
    {
        
    }

    public override void Update()
    {
     
    }

    public override void Exit()
    {
       
    }

    
}
