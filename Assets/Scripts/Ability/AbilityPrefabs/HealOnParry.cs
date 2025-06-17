using UnityEngine;

public class HealOnParry : PlayerAbility
{
    private PlayerScript player;

    public override void OnEquip(PlayerScript player)
    {
        Debug.Log("�������Ƽ ������");
        this.player = player;
        player.OnParrySuccess += HealAbility;
    }

    public override void OnUnequip(PlayerScript player)
    {
        player.OnParrySuccess -= HealAbility;
    }

    private void HealAbility()
    {
        //int amount = level switch
        //{
        //    1 => 1,
        //    2 => 2,
        //    3 => 3,
        //    _ => 0
        //};
        Debug.Log("�����Ƽ(ȸ��) �ߵ�");
        player.abilTestPlayerHealth(10);

        PlayerLogger.Instance.AddHealing(10);
    }
}
