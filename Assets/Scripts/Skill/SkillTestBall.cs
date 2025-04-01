using UnityEngine;

public class SkillTestBall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 몬스터 태그를 가진 오브젝트와 충돌했을 때
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject, 0.1f);
        }
    }
}