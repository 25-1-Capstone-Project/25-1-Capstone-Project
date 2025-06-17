using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Player/Skill/Boomerang")] 
public class Skill_boomerang : SkillPattern
{
    [SerializeField] private GameObject boomerangPrefab;
    [SerializeField] private float travelDistanceUltimate = 5f;
    [SerializeField] private float speed = 10f;

    public override IEnumerator CommonSkill(PlayerScript player)
    {
        EffectPooler.Instance.SpawnFromPool("AttackSlashParticle", player.Direction.normalized + Vector3.right, Quaternion.LookRotation(player.Direction) );
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
        SpawnBoomerang(player, travelDistanceUltimate, ultimateDamage);
        yield return null;
    }

    private void SpawnBoomerang(PlayerScript player, float distance, int damage)
    {
        Debug.Log("�θ޶�������");
        Vector2 dir = player.Direction.normalized;
        Vector3 spawnPos = player.transform.position;

        GameObject boomerang = EffectPooler.Instance.SpawnFromPool("playerAttackBoomerang", spawnPos, Quaternion.identity);

        var proj = boomerang.GetComponent<BoomerangProjectile>();
        boomerang.GetComponent<PlayerAttack>().damage = damage;
        boomerang.GetComponent<PlayerAttack>().disableOnEnemyHit = false;

        proj.Initialize(dir, distance, speed, player.transform);
    }
}
