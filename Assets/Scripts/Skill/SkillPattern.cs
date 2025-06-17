using UnityEngine;
using System.Collections;

public abstract class SkillPattern : ScriptableObject
{
 
    public int damage = 0;
    public int ultimateDamage = 0;
    public int commonCost = 0;
    public int ultimateCost = 0;
    public int cooldown = 0;
    public Sprite skillIcon;
    
    public bool ParryStackCheck()
    {
        if (PlayerScript.Instance.ParryStack >= ultimateCost)
        {
            return true;
        }
        // else if (PlayerScript.Instance.ParryStack >= commonCost)
        // {
        //     return SkillType.Common;
        // }
        return false;
    }

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