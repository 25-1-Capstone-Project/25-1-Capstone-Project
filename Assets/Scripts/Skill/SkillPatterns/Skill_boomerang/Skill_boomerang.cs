using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Player/Skill/Boomerang")] 
public class Skill_boomerang : SkillPattern
{
    [SerializeField] private GameObject boomerangPrefab;
    [SerializeField] private float travelDistanceCommon = 3f;
    [SerializeField] private float travelDistanceUltimate = 5f;
    [SerializeField] private float speed = 10f;

    public override IEnumerator CommonSkill(PlayerScript player)
    {
        SpawnBoomerang(player, travelDistanceCommon, damage);
        yield return null;
    }

    public override IEnumerator UltimateSkill(PlayerScript player)
    {
        SpawnBoomerang(player, travelDistanceUltimate, ultimateDamage);
        yield return null;
    }

    private void SpawnBoomerang(PlayerScript player, float distance, int damage)
    {
        Debug.Log("ºÎ¸Þ¶û»ý¼ºµÊ");
        Vector2 dir = player.Direction.normalized;
        Vector3 spawnPos = player.transform.position;

        GameObject boomerang = EffectPooler.Instance.SpawnFromPool("playerAttackBoomerang", spawnPos, Quaternion.identity);

        var proj = boomerang.GetComponent<BoomerangProjectile>();
        boomerang.GetComponent<PlayerAttack>().damage = damage;
        boomerang.GetComponent<PlayerAttack>().disableOnEnemyHit = false;

        proj.Initialize(dir, distance, speed, player.transform);
    }
}
