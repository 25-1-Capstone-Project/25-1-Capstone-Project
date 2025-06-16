
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileEnemyAttack : EnemyAttackBase
{
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void FixedUpdate()
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
                    gameObject.SetActive(false);
                break;
        }
    }

}
