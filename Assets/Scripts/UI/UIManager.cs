using UnityEngine;


public class UIManager : Singleton<UIManager> 
{
  
    public PlayerStatUI playerStatUI;
    public ParryStackUI parryStackUI;
    public SkillUI skillUI;
    public SkillSelect skillSelect;

    protected override void Awake()
    {
        base.Awake();
    }

    private void LateUpdate()
    {
        //playerStatUI.UI_ParryCooldownUpdate();
        //playerStatUI.UI_ParryStackUpdate();
        
    }

}
