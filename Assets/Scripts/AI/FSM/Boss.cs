
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public enum EBossSkillAction { Slash, Shot, AreaAttack, JumpSmash }

public class Boss : Enemy
{
    private Fuzzy fuzzy;
    public Text StateText;
    public Text AttackText;
    float cooldownTime = 2.0f;

    public override void Init()
    {
        enemyAnimController = GetComponent<BossAnimatorController>();
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
       
    }
    public void StartFSM()
    {
        StateMachine.ChangeState<BossIdle>();
        Debug.Log("Boss FSM Started");
    }  
    public void DecideSkill()
    {
        EBossSkillAction skilltype = fuzzy.DecideSkill(Vector2.Distance(transform.position, PlayerScript.Instance.GetPlayerTransform().position),
        data.currentHealth, PlayerScript.Instance.Health, PlayerScript.Instance.GetRigidbody().linearVelocity.magnitude);
      
        data.AttackPatternSet((int)skilltype);
        
    }

    public bool CheckCooldownComplete(float timer)
    {
        return timer >= cooldownTime;
    }
}


