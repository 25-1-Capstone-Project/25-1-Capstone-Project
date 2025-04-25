using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    public Transform skillObjectsParent;
    protected override void Awake()
    {
      
        base.Awake();
        
    }

    public SkillPattern[] SkillPatterns;
}
