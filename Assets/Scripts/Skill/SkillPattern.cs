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
    public int commonCost = 0;
    public int ultimateCost = 0;
    public int cooldown = 0;
    public Sprite skillIcon;
    
    // 패리스택 체크
    // 궁 UI 관련해서 스킬 구조에 작은 이슈가 생김(-> UI갱신관련.. 패링에다가 때려박음) => 제대로 생각해 볼 것
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

    // 스킬 쿨타임 체크
    [System.NonSerialized]
    public float lastUseTime = -Mathf.Infinity;

    public bool IsCooldownReady()
    {
        return Time.unscaledTime >= lastUseTime + cooldown;
    }

    public void SetCooldown()
    {
        lastUseTime = Time.unscaledTime;
    }

    //쿨다운 리셋...
    public void ResetCooldown()
    {
        lastUseTime = lastUseTime - cooldown;
    }



    public abstract IEnumerator CommonSkill(PlayerScript player);
    public abstract IEnumerator UltimateSkill(PlayerScript player);
}