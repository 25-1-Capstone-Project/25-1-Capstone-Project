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
    public float fireballSpeed = 0;
    public int damage = 0;
    public int commonCost = 0;
    public int ultimateCost = 0;
    public SkillType ParryStackCheck()
    {
        if (PlayerScript.Instance.ParryStack >= ultimateCost)
        {
            return SkillType.Ultimate;
        }
        else if (PlayerScript.Instance.ParryStack >= commonCost)
        {
            return SkillType.Common;
        }
        return SkillType.Empty;
    }
   
    public abstract IEnumerator CommonSkill(PlayerScript player);
    public abstract IEnumerator UltimateSkill(PlayerScript player);
}