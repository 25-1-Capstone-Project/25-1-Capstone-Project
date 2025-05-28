
using UnityEngine;

/// <summary>
/// 플레이어 충돌 처리
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    int damage;
    float speed = 10f; // 속도 설정
    Rigidbody2D rb;
    Vector2 directionVec;
    public int GetDamage() => damage;
    public void SetDamage(int damage) => this.damage = damage;
    public Vector2 GetDirectionVec() => PlayerScript.Instance.GetPlayerTransform().position - transform.position;
    public Vector2 GetDirectionNormalVec() => GetDirectionVec().normalized;
    public PlayerAttack playerAttack;

    public void PlayerAttackSet()
    {
        playerAttack.SetDamage(this.GetDamage());
    }

    public void SetDirectionVec(Vector2 direction)
    {
        // 방향 벡터 저장
        directionVec = direction.normalized;

        // 방향을 기준으로 회전 (오른쪽이 앞일 경우)
        transform.rotation = Quaternion.FromToRotation(Vector3.right, directionVec);

    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAttack = GetComponent<PlayerAttack>();
        PlayerAttackSet();
    }
    public void FixedUpdate()
    {
        rb.linearVelocity = (Vector2)transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            gameObject.SetActive(false); // 충돌 후 오브젝트 비활성화
        }
    }

}
