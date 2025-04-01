using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/SwordSlash")]
public class SwordSlashAttack : EnemyAttackPattern
{
    public int damage = 7;
    public float delay = 0.2f;
    public float range = 1f;

    public override IEnumerator Execute(Enemy enemy)
    {
        enemy.IsAttacking = true;

        yield return new WaitForSeconds(delay);

        Vector2 direction = (enemy.GetPlayer().position - enemy.transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, direction, range, LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {
            
            hit.collider.GetComponent<Player>()?.TakeDamage(damage, direction, enemy);
        }

        yield return new WaitForSeconds(0.5f);
        enemy.IsAttacking = false;
    }
}
