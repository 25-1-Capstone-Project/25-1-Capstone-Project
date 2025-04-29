// 2025.04.29 안예선
// 스킬B(강타!)를 관리하는 스크립트입니다. (스킬 설명: 전방 부채꼴 n˚각도로 세게 공격 / 플레이어 주위를 세게 공격)
// 사용 시 현재 상태에 맞는 함수가 호출되어(Common/Ultimate) 적 대미지 판정을 처리합니다.
//
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
