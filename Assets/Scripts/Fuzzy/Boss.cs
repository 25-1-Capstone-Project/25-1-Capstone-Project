using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class Boss : MonoBehaviour
{
    public Transform player; // 플레이어의 위치
    public float attackRange = 3f; // 공격 범위
    public float moveSpeed = 2f; // 이동 속도
    private FSM _fsm;
    public float health = 100; // 보스의 체력


    private void Start()
    {
        _fsm = new FSM(new Idle(this));
    }
    private void Update()
    {
        _fsm.UpdateState();

        if (health <= 0 && !(_fsm.CurrentState is Dead))
        {
            ChangeState(new Dead(this));
        }
    }

    public void ChangeState(BaseState state) => _fsm.ChangeState(state);
    public abstract bool IsPlayerInRange();
    public void MoveToPlayer()
    {
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        transform.position = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;
    }

}

