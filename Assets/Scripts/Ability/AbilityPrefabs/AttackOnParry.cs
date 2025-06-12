using UnityEngine;

public class AttackOnParry : PlayerAbility
{
    private PlayerScript player;

    public float attackRange = 3f;
    public int damage = 5;
    LayerMask enemyLayer = 6; // 적 레이어... 추후개선요망

    public override void OnEquip(PlayerScript player)
    {
        Debug.Log("광역공격 어빌리티 장착됨");
        this.player = player;
        player.OnParrySuccess += AttackAbility;
    }

    public override void OnUnequip(PlayerScript player)
    {
        player.OnParrySuccess -= AttackAbility;
    }

    private void AttackAbility()
    {
        Debug.Log("어빌리티(광역딜)발동");
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, attackRange, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            Debug.Log("어빌리티: 때림");
            hit.GetComponent<EnemyBase>().TakeDamage(damage);
        }
    }

    // 돌진 몹 돌진 판정때문에 딜이 안 들어가는 문제? 확인필요
    // 뱀 몹 대미지 판정 확인 필요
}