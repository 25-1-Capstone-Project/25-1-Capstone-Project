
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public enum EBossSkillAction { Slash, Shot, AreaAttack, JumpSmash }

public class Boss : Enemy
{
    private Fuzzy fuzzy;
    public Text StateText;
    public Text AttackText;
    EBossSkillAction skilltype;
    BossAnimatorController bossAnimController;
  
    public override void Init(){
        bossAnimController = GetComponent<BossAnimatorController>();
     
        fuzzy = new Fuzzy();
     
    }
    
    protected override void SetState()
    {
        StateMachine = new StateMachine<IState>();
        StateMachine.AddState(new BossIdle(this));
        StateMachine.AddState(new BossPreparing(this));
        StateMachine.AddState(new BossAttack(this));
        StateMachine.AddState(new BossCooldown(this));
        StateMachine.AddState(new BossMove(this));
        StateMachine.AddState(new BossDead(this));
        StateMachine.ChangeState<BossIdle>();
    }
   
    public void DecideSkill()
    {

        EBossSkillAction skilltype = fuzzy.DecideSkill(Vector2.Distance(transform.position, PlayerScript.Instance.GetPlayerTransform().position),
        data.currentHealth, PlayerScript.Instance.Health, PlayerScript.Instance.GetRigidbody().linearVelocity.magnitude);
        AttackText.text = skilltype.ToString();
    }



}


