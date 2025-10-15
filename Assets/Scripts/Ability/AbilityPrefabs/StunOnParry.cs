using UnityEngine;

public class StunOnParry : PlayerAbility
{
    private PlayerScript player;

    public float attackRange = 3f;
    LayerMask enemyLayer = 6; 

    public override void OnEquip(PlayerScript player)
    {
        Debug.Log("���� �����Ƽ ������");
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
           
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
               // enemy.Parried();
            }
        }
    }
}
