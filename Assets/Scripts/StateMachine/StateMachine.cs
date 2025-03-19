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
}
