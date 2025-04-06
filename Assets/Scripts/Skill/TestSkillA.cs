using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Player/Skill/SkillA")]
public class TestSkillA : SkillPattern
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed = 10f;
    [SerializeField] private int damage = 5;
    [SerializeField] private int requiredParryStack = 1;

    public override IEnumerator Act(Player player)
    {
        if (player.ParryStack < requiredParryStack)
        {
            Debug.Log("패리 스택 부족");
            yield break;
        }

        if (fireballPrefab == null)
        {
            yield break;
        }

        player.ParryStack -= requiredParryStack;

        GameObject fireball = Instantiate(fireballPrefab, player.transform.position, Quaternion.identity,SkillManager.instance.skillObjectsParent);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = player.Direction * fireballSpeed;
        }

        yield return null;
    }
}
