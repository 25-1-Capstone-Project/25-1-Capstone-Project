using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Player/Skill/SkillC")]
public class Skill_Dash : SkillPattern
{
    [SerializeField] float dashDistance = 4f;
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float hitRadius = 0.5f;

    public override IEnumerator CommonSkill(PlayerScript player)
    {
        Vector2 start = player.transform.position;
        Vector2 direction = player.Direction.normalized;
        Vector2 target = start + direction * dashDistance;

        while ((Vector2)player.transform.position != target)
        {
            Vector2 newPos = Vector2.MoveTowards(player.transform.position, target, dashSpeed * Time.deltaTime);
            player.transform.position = newPos;

            Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, hitRadius, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits)
            {
                hit.GetComponent<EnemyBase>()?.TakeDamage(damage);
            }

            yield return null;
        }
    }

    public override IEnumerator UltimateSkill(PlayerScript player)
    {
        Vector2 start = player.transform.position;
        Vector2 direction = player.Direction.normalized;
        Vector2 target = start + direction * dashDistance;

        while ((Vector2)player.transform.position != target)
        {
            Vector2 newPos = Vector2.MoveTowards(player.transform.position, target, dashSpeed * Time.deltaTime);
            player.transform.position = newPos;

            Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, hitRadius, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits)
            {
                hit.GetComponent<EnemyBase>()?.TakeDamage((damage + 10));
            }

            yield return null;
        }

    }
}
