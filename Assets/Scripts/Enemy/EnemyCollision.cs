using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "PlayerAttack":
                PlayerAttack playerAttack = other.GetComponent<PlayerAttack>();
                playerAttack.gameObject.SetActive(false);

                // 적에게 데미지 적용
                GetComponent<EnemyBase>().Stamina--;
                break;

        }


    }
}
