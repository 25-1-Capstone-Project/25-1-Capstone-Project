using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    //추후 SkillData 추가해서 스크립터블 오브젝트 상속/SkillData 참조하는 방식으로
    //[SerializeField] protected SkillData skillData;

    [SerializeField] protected float skillCooldown;
    [SerializeField] protected int skillCost;
    [SerializeField] protected int skillDamage;

    public abstract void Act();
}
