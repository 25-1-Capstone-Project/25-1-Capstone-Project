using System;
using System.Collections.Generic;

public class StateMachine<T> where T : IState
{
    private T currentState;
    private Dictionary<Type, T> states = new Dictionary<Type, T>();

    public void AddState(T state)
    {
        states[state.GetType()] = state;
    }

    public void ChangeState<TState>() where TState : T
    {
     
        currentState?.Exit();

        currentState = states[typeof(TState)];
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void FixedUpdate()
    {
        // FixedUpdate가 필요한 상태만 실행
        if (currentState is IFixedUpdateState fixedState)
        {
            fixedState.FixedUpdate();
        }
    }

    public void LateUpdate()
    {
        // FixedUpdate가 필요한 상태만 실행
        if (currentState is ILateUpdateState lateState)
        {
            lateState.LateUpdate();
        }
    }
}
