using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Player/Skill/SkillA")]
public class TestSkillA : SkillPattern
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed;

    public override IEnumerator CommonSkill(PlayerScript player)
    {
        SpawnFireball(player.Direction, player);
        yield return null;
    }

    public override IEnumerator UltimateSkill(PlayerScript player)
    {
        SpawnFireball(Quaternion.Euler(0, 0, 30f) * player.Direction, player);
        SpawnFireball(Quaternion.Euler(0, 0, -30f) * player.Direction, player);
        SpawnFireball(player.Direction, player);


        yield return null;
    }

    private void SpawnFireball(Vector2 direction, PlayerScript player)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject fireball = Instantiate(fireballPrefab, player.transform.position, rotation, SkillManager.Instance.skillObjectsParent);
        fireball.GetComponent<Rigidbody2D>().linearVelocity = direction * fireballSpeed;

        fireball.GetComponent<PlayerAttack>().damage = damage; 
    }

}
