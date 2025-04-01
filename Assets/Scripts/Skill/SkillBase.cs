using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    //���� SkillData �߰��ؼ� ��ũ���ͺ� ������Ʈ ���/SkillData �����ϴ� �������
    //[SerializeField] protected SkillData skillData;

    [SerializeField] protected float skillCooldown;
    [SerializeField] protected int skillCost;
    [SerializeField] protected int skillDamage;

    public abstract void Act();
}
