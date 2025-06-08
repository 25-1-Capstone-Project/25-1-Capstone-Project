
using UnityEngine;

public class EnemyAttackBase : MonoBehaviour
{
    protected int damage;
    [SerializeField] protected bool canParry = true;  // 플레이어가 공격을 막을 수 있는지 여부
    public bool CanParry => canParry; // 외부에서 접근할 수 있는 프로퍼티
    public void SetCanParry(bool canParry) => this.canParry = canParry;

    protected float speed = 10f; // 속도 설정
    Vector2 directionVec;
    public int GetDamage() => damage;
    public void SetDamage(int damage) => this.damage = damage;
    public Vector2 GetDirectionVec() => PlayerScript.Instance.GetPlayerTransform().position - transform.position;
    public Vector2 GetDirectionNormalVec() => GetDirectionVec().normalized;
    public PlayerAttack playerAttack;

    public void PlayerAttackSet()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerAttack?.SetDamage(damage);
    }

    public void SetDirectionVec(Vector2 direction)
    {
        // 방향 벡터 저장
        directionVec = direction.normalized;
        // 방향을 기준으로 회전 (오른쪽이 앞일 경우)
        transform.rotation = Quaternion.FromToRotation(Vector3.right, directionVec);
    }


}
