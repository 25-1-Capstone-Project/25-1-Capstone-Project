
using Unity.Sentis;
using UnityEngine;

public class ProjectileEnemyAttack : EnemyAttackBase
{
    protected Rigidbody2D rb;
    protected virtual void Start()
    {
        gameObject.tag = "EnemyAttack";
        rb = GetComponent<Rigidbody2D>();

        PlayerAttackSet();
    }
    protected virtual void FixedUpdate()
    {
        rb.linearVelocity = (Vector2)transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Wall":
                    gameObject.SetActive(false);
                break;
            case "Enemy":
                if (CompareTag("PlayerAttack"))
                {
                    gameObject.SetActive(false);
                    AudioManager.Instance.PlaySFX("EnemyHit");
                }
                break;
        }
    }

}
