using UnityEngine;

public class nonPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 가져오기
    }

    void Update()
    {
        // 키 입력으로 방향 결정
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float moveY = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        moveDirection = new Vector2(moveX, moveY).normalized; // 방향 벡터
    }

    void FixedUpdate()
    {
        // 물리 기반 이동
        rb.linearVelocity = moveDirection * moveSpeed; // Rigidbody2D의 속도 설정
    }
}
