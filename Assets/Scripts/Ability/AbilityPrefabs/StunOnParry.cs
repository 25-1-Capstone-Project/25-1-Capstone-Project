using UnityEngine;

public class StunOnParry : PlayerAbility
{
    private PlayerScript player;

    public float attackRange = 3f;
    LayerMask enemyLayer = 6; // 적 레이어... 추후개선요망

    public override void OnEquip(PlayerScript player)
    {
        Debug.Log("스턴 어빌리티 장착됨");
        this.player = player;
        player.OnParrySuccess += StunAbility;
    }

    public override void OnUnequip(PlayerScript player)
    {
        player.OnParrySuccess -= StunAbility;
    }

    private void StunAbility()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, attackRange, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            // 스턴인데 일단 패링효과로 짬처리...
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.Parried();
            }
        }
    }
}
