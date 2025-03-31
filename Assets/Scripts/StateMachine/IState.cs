using UnityEngine;

// 상태 인터페이스 정의
public interface IState
{
    void Enter();  
    void Update();  
    void Exit();   
}
public interface IFixedUpdateState
{
    void FixedUpdate();
}