using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    public enum State
    {
        Idle,
        Preparing,
        Attack,
        Cooldown,
        Move,
        Dead
    }

    public float health = 100f;
    public float moveSpeed = 5f;

    public float playerHp = 100f; // 플레이어 HP (BossA에서 할당해줘야 함)

    public float playerVelocity = 0f; // 플레이어 속도 (BossA에서 할당해줘야 함)

    // 공격 사정거리
    public float attackRange = 3f;

    // 플레이어 Transform (BossA에서 할당해줘야 함)
    protected Transform player;

    // 플레이어가 공격 범위 내에 있는지 여부 (자식 클래스에서 구현)

    // 플레이어 참조 세팅용 (필요 시 호출)
    public virtual void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public abstract void ChangeState(BaseState nextState);


    private State _state;
    private void Start()
    {
        _state = State.Idle;
    }
    private void Update()
    {
        switch (_state)
        {
            case State.Idle:
                // Idle logic
                break;
            case State.Preparing:
                // Preparing logic
                break;
            case State.Attack:
                // Attack logic
                break;
            case State.Cooldown:
                // Cooldown logic
                break;
            case State.Move:
                // Move logic
                break;
            case State.Dead:
                // Dead logic
                break;
        }
        // Example of state transition
    }

    public virtual void MoveToPlayer()
    {
        if (player == null) return;

        // 현재 위치에서 플레이어 위치 방향으로 이동
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 필요 시 보스가 플레이어를 바라보게 회전도 가능
        // transform.forward = direction; // 3D 게임일 경우
        // 2D 게임이면 아래처럼 z축 회전 조정 가능
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public virtual bool IsPlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }
    
    
}
