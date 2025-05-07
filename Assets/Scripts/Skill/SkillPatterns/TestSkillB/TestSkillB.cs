using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Player/Skill/SkillB")]
public class TestSkillB : SkillPattern
{
    [SerializeField] float attackRange;
    [SerializeField] float attackAngle;
    [SerializeField] GameObject attackVFXPrefab;

    public override IEnumerator CommonSkill(PlayerScript player)
    {
        float VFXangle = Mathf.Atan2(player.Direction.y, player.Direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, VFXangle);
        GameObject effect = EffectPooler.Instance.SpawnFromPool("AttackSlashParticle", player.transform.position, rotation);

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
        if (attackVFXPrefab != null)
        {
            GameObject vfx = GameObject.Instantiate(attackVFXPrefab, player.transform.position, Quaternion.identity);
        }

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
