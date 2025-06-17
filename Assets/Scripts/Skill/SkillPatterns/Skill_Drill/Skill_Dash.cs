using UnityEngine;
using System.Collections;
using Unity.Mathematics;

[CreateAssetMenu(menuName = "Player/Skill/SkillC")]
public class Skill_Dash : SkillPattern
{
    [SerializeField] float dashDistance = 4f;
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float hitRadius = 0.5f;

    public override IEnumerator CommonSkill(PlayerScript player)
    {
        float temp = Mathf.Atan2(player.Direction.y, player.Direction.x) * Mathf.Rad2Deg;
        Quaternion dir = Quaternion.Euler(0, 0, temp);
        
        EffectPooler.Instance.SpawnFromPool("AttackSlashParticle", (Vector2)(player.transform.position + player.Direction.normalized), dir);
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, player.Stats.attackRange, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            Vector2 toTarget = (hit.transform.position - player.transform.position).normalized;
            float angle = Vector2.Angle(player.Direction, toTarget);

            if (angle <= player.Stats.attackAngle / 2f)
            {
                hit.GetComponent<EnemyBase>()?.TakeDamage(player.Stats.damage);
            }
        }

        yield return new WaitForSeconds(player.Stats.attackCooldownSec);

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
