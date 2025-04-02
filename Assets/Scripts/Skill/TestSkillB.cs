using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Player/Skill/SkillB")]
public class TestSkillB : SkillPattern
{
    [SerializeField] private int damage = 10;
    [SerializeField] private int requiredParryStack = 2;
    [SerializeField] private float attackRadius = 2f;

    public override IEnumerator Act(Player player)
    {
        if (player.ParryStack < requiredParryStack)
        {
            Debug.Log("패리 스택 부족");
            yield break;
        }

        player.ParryStack -= requiredParryStack;


        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(player.transform.position, attackRadius);
        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Enemy"))
            {
                // 몬스터 대미지 처리 고민좀...
                Debug.Log("공격(스킬B)");
            }
        }

        Debug.Log($"스킬B / 대미지: {damage}, 남은 패리 스택: {player.ParryStack}");

        yield return null;
    }
}
