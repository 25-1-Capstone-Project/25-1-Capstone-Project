using UnityEngine;

public class Enemy : MonoBehaviour
{
    public StateMachine<MonsterState> StateMachine { get; private set; }

    void Start()
    {
        StateMachine = new StateMachine<MonsterState>();
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new MoveState(this));
        StateMachine.AddState(new AttackState(this));

        //init
        StateMachine.ChangeState<IdleState>();
    }

    void Update()
    {
        StateMachine.Update();
    }
}
