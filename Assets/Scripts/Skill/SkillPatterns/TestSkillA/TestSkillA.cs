using UnityEngine;
using System.Collections;

//[CreateAssetMenu(menuName = "Player/Skill/SkillA")]
public class TestSkillA : SkillPattern
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed;

    public override IEnumerator CommonSkill(PlayerScript player)
    {
        SpawnFireball(player.Direction, player, damage);
        yield return null;
    }

    public override IEnumerator UltimateSkill(PlayerScript player)
    {
        SpawnFireball(Quaternion.Euler(0, 0, 30f) * player.Direction, player, ultimateDamage);
        SpawnFireball(Quaternion.Euler(0, 0, -30f) * player.Direction, player, ultimateDamage);
        SpawnFireball(player.Direction, player, ultimateDamage);
        
        yield return null;
    }

    private void SpawnFireball(Vector2 direction, PlayerScript player, int damage)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject fireball = EffectPooler.Instance.SpawnFromPool("playerAttackFireball", player.transform.position, rotation);

        fireball.GetComponent<Rigidbody2D>().linearVelocity = direction * fireballSpeed;
        fireball.GetComponent<PlayerAttack>().damage = damage;
        fireball.GetComponent<PlayerAttack>().disableOnEnemyHit = true;
    }

}
