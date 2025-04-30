using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Player/Skill/SkillB")]
public class TestSkillB : SkillPattern
{
    [SerializeField] float attackRange;
    [SerializeField] float attackAngle;
    [SerializeField] float attackVFX;

    public override IEnumerator CommonSkill(PlayerScript player)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, attackRange, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            if (hit != null)
            {
                Vector2 toTarget = (hit.transform.position - player.transform.position).normalized;
                float angle = Vector2.Angle(player.Direction, toTarget);

                if (angle <= attackAngle / 2f)
                {
                    hit.GetComponent<Enemy>()?.TakeDamage(damage);
                }
            }
        }

        yield return null;
    }

    public override IEnumerator UltimateSkill(PlayerScript player)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, attackRange, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            if (hit != null)
            {
                hit.GetComponent<Enemy>().TakeDamage(damage + 10);
            }
        }

        yield return null;
    }
}
