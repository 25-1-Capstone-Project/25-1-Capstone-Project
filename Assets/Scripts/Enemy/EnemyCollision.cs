using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    EnemyBase enemy;
    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "PlayerAttack":
                PlayerAttack playerAttack = other.GetComponent<PlayerAttack>();
                playerAttack.gameObject.SetActive(false);

                // 적에게 데미지 적용
                enemy.Stamina--;
                break;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Wall":
            //  case "Hole":
            case "Enemy":
                if (!enemy.thrownEnenmy) break;
                enemy.GetRigidbody().linearVelocity = Vector2.zero;

                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2f, LayerMask.GetMask("Enemy"));
                foreach (Collider2D collider in colliders)
                {
                    EnemyBase enemyComponent = collider.GetComponent<EnemyBase>();
                    if (enemy != enemyComponent && enemyComponent != null)
                    {
                        enemyComponent.Stamina--;
                    }
                }
                EffectPooler.Instance.SpawnFromPool("ThrowImpact", transform.position);
                enemy.thrownEnenmy = false;
                enemy.TakeDamage(1);
                break;
        }
    }
}
