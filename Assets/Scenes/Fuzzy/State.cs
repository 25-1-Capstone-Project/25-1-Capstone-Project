using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public abstract class BaseState
{
    protected Boss _boss;

    public BaseState(Boss monster)
    {
        _boss = monster;
    }
    
    public virtual void Enter() { } //상태 진입 시 호출
    public virtual void Update() { } //매 프레임 마다 호출
    public virtual void Exit() { } // 상태가 변경되면 호출
}

public class Idle : BaseState
{
    private float _timer = 0f;
    public Idle(Boss monster) : base(monster) { }

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
            _boss.ChangeState(new Preparing(_boss));
        }
        Debug.Log("In Idle State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}
public class Preparing : BaseState
{
    private float _timer = 0f;
    private float prepareTime = 2f; // 준비 시간
    public bool IsPreparingComplete() => _timer >= prepareTime;
    public Preparing(Boss monster) : base(monster) { }

    public override void Enter()
    {
        _timer = 0f;
        Debug.Log("Entering Preparing State");
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        Debug.Log("In Preparing State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Preparing State");
    }
}
public class Attack : BaseState
{
    private float _timer;
    private float attackTime = 3f; // 공격 시간
    public Attack(Boss monster) : base(monster) { }

    public override void Enter()
    {
        _timer = 0f;
        Debug.Log("Entering Attack State");
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        Debug.Log("In Attack State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Attack State");
    }

    public bool IsAttackFinished()
    {
        return _timer >= attackTime;
    }
}

public class Cooldown : BaseState
{
    private float cooldownTime = 2.0f;
    private float _timer = 0f;
    public bool IsCooldownComplete => _timer >= cooldownTime;
    public Cooldown(Boss monster) : base(monster) { }

    public override void Enter()
    {
        _timer = 0f;
        Debug.Log("Entering Cooldown State");
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        Debug.Log("In Cooldown State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Cooldown State");
    }
}

public class Move : BaseState
{
    public Move(Boss monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("Entering Move State");
    }

    public override void Update()
    {
        Debug.Log("In Move State");
    }

    public override void Exit()
    {
        _boss.MoveToPlayer();
        Debug.Log("Exiting Move State");
    }
}

public class Dead : BaseState
{
    public Dead(Boss monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("Entering Dead State");
    }

    public override void Update()
    {
        Debug.Log("In Dead State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Dead State");
    }

    
}
