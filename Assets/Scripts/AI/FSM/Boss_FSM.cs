using NUnit.Framework;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class Boss_FSM : Boss
{
    public enum State { Idle, Preparing, Attack, Cooldown, Move, Dead }

    private State _curState;
    private FSM _fsm;
    private Fuzzy fuzzy;
    public Text StateText;
    public Text AttackText;


    private void Start()
    {
        _curState = State.Idle;
        _fsm = new FSM(new Idle(this));
        fuzzy = new Fuzzy();
    }

    private void Update()
    {
        StateText.text = _fsm.CurrentState.GetType().Name;
        AttackText.text = fuzzy.DecideSkillFuzzy(Vector2.Distance(transform.position, player.position), health).ToString();

        switch (_curState)
        {
            case State.Idle:
                if (IsPlayerInRange())
                {
                    ChangeState(State.Preparing);
                }
                else
                {
                    ChangeState(State.Move);
                }
                break;
            case State.Preparing:
                if (_fsm.CurrentState is Preparing preparingState && preparingState.IsPreparingComplete())
                {
                    ChangeState(State.Attack);
                }
                break;
            case State.Attack:
                // 보스의 체력과 거리 정보를 기반으로 스킬 결정
                SkillAction action = fuzzy.DecideSkillFuzzy(Vector2.Distance(transform.position, player.position), health);
                switch (action)
                {
                    case SkillAction.Slash:
                        // Slash 스킬 실행
                        Debug.Log("Slash");
                        break;
                    case SkillAction.Dash:
                        // Dash 스킬 실행
                        Debug.Log("Dash");
                        break;
                    case SkillAction.Shot:
                        // Shot 스킬 실행
                        Debug.Log("Shot");
                        break;
                    case SkillAction.AreaAttack:
                        // AreaAttack 스킬 실행
                        Debug.Log("AreaAttack");
                        break;
                    case SkillAction.JumpSmash:
                        // JumpSmash 스킬 실행
                        Debug.Log("JumpSmash");
                        break;
                    default:
                        break;
                }
                if (_fsm.CurrentState is Attack attackState && attackState.IsAttackFinished())
                {
                    ChangeState(State.Cooldown);
                }
                break;
            case State.Cooldown:
                if (_fsm.CurrentState is Cooldown cooldownState && cooldownState.IsCooldownComplete)
                {
                    ChangeState(State.Idle);
                }
                break;
            case State.Move:
                if (IsPlayerInRange())
                {
                    ChangeState(State.Preparing);
                }
                else
                {
                    MoveToPlayer();
                }
                break;
            case State.Dead:
                //Dead
                break;
        }
        if (health <= 0 && !(_fsm.CurrentState is Dead))
        {
            ChangeState(State.Dead);
        }
        _fsm.UpdateState();

    }
    private void ChangeState(State nextState)
    {
        _curState = nextState;
        switch (nextState)
        {
            case State.Idle:
                _fsm.ChangeState(new Idle(this));
                break;
            case State.Preparing:
                _fsm.ChangeState(new Preparing(this));
                break;
            case State.Attack:
                _fsm.ChangeState(new Attack(this));
                break;
            case State.Cooldown:
                _fsm.ChangeState(new Cooldown(this));
                break;
            case State.Move:
                _fsm.ChangeState(new Move(this));
                break;
            case State.Dead:
                _fsm.ChangeState(new Dead(this));
                break;
        }
    }

    public override bool IsPlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }
    
    public override void ChangeState(BaseState newState)
    {
        _fsm.ChangeState(newState);
    }

}


