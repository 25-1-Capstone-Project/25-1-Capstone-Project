using UnityEngine;

// 몬스터 상태 기본 클래스
public abstract class MonsterState : IState
{
    protected Enemy monster;

    public MonsterState(Enemy monster)
    {
        this.monster = monster;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

// Idle 상태
public class IdleState : MonsterState
{
    public IdleState(Enemy monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("몬스터 대기 중");
    }

    public override void Update()
    {
 
    }

    public override void Exit()
    {
        Debug.Log("몬스터 이동 시작");
    }
}

// 추격 상태
public class MoveState : MonsterState
{
    public MoveState(Enemy monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("몬스터 추격 시작");
    }

    public override void Update()
    {
    
    }

    public override void Exit()
    {
        Debug.Log("몬스터 추격 종료");
    }
}

// 공격 상태
public class AttackState : MonsterState
{
    public AttackState(Enemy monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("몬스터 공격!");
    }

    public override void Update()
    {
   
    }

    public override void Exit()
    {
        Debug.Log("몬스터 공격 종료");
    }
}
