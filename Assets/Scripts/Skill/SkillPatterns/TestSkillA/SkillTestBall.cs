// 2025.04.29 안예선
// 파이어볼 프리팹에 붙어 파이어볼 객체의 파괴와 대미지 처리를 담당하는 스크립트입니다.
// 

using UnityEngine;

public class SkillTestBall : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 벽·오브젝트 충돌할 경우 추가 必
        // if (other.CompareTag("Wall") 
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject, 0.1f);
        }
    }
}