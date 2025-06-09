using UnityEngine;
using System.Collections;

public enum SkillType
{
    Empty=-1,
    Common,
    Ultimate,
}
public abstract class SkillPattern : ScriptableObject
{
    public SkillType skillType = SkillType.Common;
    public int damage = 0;
    public int ultimateDamage = 0;
    public int commonCost = 0;
    public int ultimateCost = 0;
    public int cooldown = 0;
    public Sprite skillIcon;
    
    // �и����� üũ
    public SkillType ParryStackCheck()
    {
        if (PlayerScript.Instance.ParryStack >= ultimateCost)
        {
            return SkillType.Ultimate;
        }
        // else if (PlayerScript.Instance.ParryStack >= commonCost)
        // {
        //     return SkillType.Common;
        // }
        return SkillType.Empty;
    }

    // ��ų ��Ÿ�� üũ
    [System.NonSerialized]
    public float lastUseTime = -Mathf.Infinity;

    public bool IsCooldownReady()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    public void SetCooldown()
    {
        lastUseTime = Time.time;
    }

    //��ٿ� ����...
    public void ResetCooldown()
    {
        lastUseTime = lastUseTime - cooldown;
    }



    public abstract IEnumerator CommonSkill(PlayerScript player);
    public abstract IEnumerator UltimateSkill(PlayerScript player);
}