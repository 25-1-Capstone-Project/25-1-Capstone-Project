using UnityEngine;

/// <summary>
/// 플레이어 충돌 처리
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    int damage;
    public int GetDamage() => damage;
    public void SetDamage(int damage) => this.damage = damage;
    public Vector2 GetDirectionVec() => PlayerScript.Instance.GetPlayerTransform().position - transform.position;
    public Vector2 GetDirectionNormalVec() => GetDirectionVec().normalized;

    

}
