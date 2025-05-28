using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "PlayerAttack":
                PlayerAttack playerAttack = other.GetComponent<PlayerAttack>();
                int damage = playerAttack.GetDamage();
                playerAttack.gameObject.SetActive(false); // 플레이어 공격 오브젝트 비활성화

                // 적에게 데미지 적용
                GetComponent<Enemy>().TakeDamage(damage);
                break;

        }
   

    }
}
