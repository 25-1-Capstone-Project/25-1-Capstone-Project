using UnityEngine;


public abstract class BaseState
{
    protected Monster _monster;

    public BaseState(Monster monster)
    {
        _monster = monster;
    }
    
    public virtual void Enter() { } //상태 진입 시 호출
    public virtual void Update() { } //매 프레임 마다 호출
    public virtual void Exit() { } // 상태가 변경되면 호출
}

public class IdleState : BaseState
{
    public IdleState(Monster monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
    }

    public override void Update()
    {
        // Logic for idle state
        Debug.Log("In Idle State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}
public class rangedAttack : BaseState
{
    public rangedAttack(Monster monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("Entering Move State");
    }

    public override void Update()
    {
        _monster.Move();
        Debug.Log("In Move State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Move State");
    }
}
public class meleeAttack : BaseState
{
    public meleeAttack(Monster monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("Entering Attack State");
    }

    public override void Update()
    {
        // Logic for attack state
        Debug.Log("In Attack State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Attack State");
    }
}

public class Move : BaseState
{
    public Move(Monster monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("Entering Attack State");
    }

    public override void Update()
    {
        // Logic for attack state
        Debug.Log("In Attack State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Attack State");
    }
}


public class FSM
{
    public BaseState _curState;
    public BaseState nextState;
    public FSM(BaseState initState)
    {
        _curState = initState;
        ChangeState(_curState);
    }

    public void ChangeState(BaseState newState)
    {
        if (nextState == _curState) return;

        if (_curState != null) _curState.Exit();

        _curState = newState;
        _curState.Enter();
    }

    public void UpdateState()
    {
        if (_curState != null) _curState.Update();
    }
}

public abstract class Monster : MonoBehaviour
{
    protected FSM _fsm;
    public abstract void Move();
    public abstract void Attack();
    public abstract void Idle();


    protected virtual void Start()
    {
        _fsm = new FSM(new IdleState(this)); // 초기 상태 설정
    }

    protected virtual void Update()
    {
        _fsm.UpdateState();
    }

    public void ChangeState(BaseState newState)
    {
        _fsm.ChangeState(newState);
    }
}



