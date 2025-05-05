using JetBrains.Annotations;
using UnityEngine;
public class FSM
{
    private BaseState _curState;
    
    public FSM(BaseState initialState)
    {
        _curState = initialState;
        _curState?.Enter();
    }

    public void ChangeState(BaseState nextState)
    {
        if (nextState == null || nextState == _curState)
            return;

        _curState?.Exit();
        _curState = nextState;
        _curState.Enter();
    }

    public void UpdateState()
    {
        _curState?.Update();
    }

    public BaseState CurrentState => _curState;

}