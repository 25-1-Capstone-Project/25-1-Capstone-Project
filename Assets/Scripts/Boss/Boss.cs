
using UnityEngine;
using UnityEngine.UI;
public enum EBossSkillAction { Slash, Shot, AreaAttack, JumpSmash }

public class Boss : Enemy
{
    private Fuzzy fuzzy;
    public Text StateText;
    public Text AttackText;
    [SerializeField] float cooldownTime = 2.0f;

    public override void Init(bool autoStartState = false)
    {
        base.InitShared();
        SetState();
        
        fuzzy = new Fuzzy();

    }
    protected override void SetState()
    {
        StateMachine = new StateMachine<IEnemyState>();
        StateMachine.AddState(new BossIdle(this));
        StateMachine.AddState(new BossAttack(this));
        StateMachine.AddState(new BossCooldown(this));
        StateMachine.AddState(new BossMove(this));
        StateMachine.AddState(new BossDead(this));
        StateMachine.AddState(new DamagedState(this));
    
        StartFSM();
    }
    public void StartFSM()
    {
        StateMachine.ChangeState<BossIdle>();
        ((BossAnimatorController)animController).PlayStartBattle();
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


