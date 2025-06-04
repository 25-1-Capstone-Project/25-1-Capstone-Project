
using UnityEngine;
using UnityEngine.UI;
public enum EBossSkillAction { Slash, Shot, AreaAttack, JumpSmash }

public enum EBoss_State { Idle, Preparing, Attack, Cooldown, Move, Dead }
public class Boss : MonoBehaviour
{
    [SerializeField] BossData bossData;
    private EBoss_State _curState;
    private Fuzzy fuzzy;
    public Text StateText;
    public Text AttackText;

    public StateMachine<BossState> StateMachine { get; private set; }

    private void Start()
    {
        SetStateMachine();
        fuzzy = new Fuzzy();
 

    }
    private void SetStateMachine()
    {
        StateMachine = new StateMachine<BossState>();
        StateMachine.AddState(new BossIdle(this));
        StateMachine.AddState(new BossPreparing(this));
        StateMachine.AddState(new BossAttack(this));
        StateMachine.AddState(new BossCooldown(this));
        StateMachine.AddState(new BossMove(this));
        StateMachine.AddState(new BossDead(this));
        StateMachine.ChangeState<BossIdle>();
    }

    private void Update()
    {
        StateText.text = _curState.ToString();
        EBossSkillAction skilltype = fuzzy.DecideSkill(Vector2.Distance(transform.position, PlayerScript.Instance.GetPlayerTransform().position),
        bossData.currentHealth, PlayerScript.Instance.Health, PlayerScript.Instance.GetRigidbody().linearVelocity.magnitude);
        AttackText.text = skilltype.ToString();
    }



}


