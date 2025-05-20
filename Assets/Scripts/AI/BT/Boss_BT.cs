using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Boss_BT : MonoBehaviour
{
    public Transform target;
    public Text currentSkillText;
    public Text currentHpText;
    public Text currentDistanceText;
    public float moveSpeed = 2f;

    bool IsAnimationRunning = false;
    bool isAttackTurn = true;
    float currentDistance;

    Rigidbody2D rb;
    Vector2 dir;

    SelectorNode rootNode;
    SequenceNode attackSequence;
    SequenceNode moveSequence;

    Fuzzy fuzzy = new Fuzzy();
    SkillAction currentSkill = SkillAction.Slash;
    float currentHp = 100f;

      
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 공격 시퀀스
        attackSequence = new SequenceNode();
        attackSequence.Add(new ActionNode(CheckPhase));
        attackSequence.Add(new ActionNode(CheckAttackRange));
        attackSequence.Add(new ActionNode(IsAttacking));
        attackSequence.Add(new ActionNode(attackSelector_Evaluate));
        

        // 이동 시퀀스
        moveSequence = new SequenceNode();
        moveSequence.Add(new ActionNode(IsMoving));
        moveSequence.Add(new ActionNode(move));

        // 루트 노드
        rootNode = new SelectorNode();
        rootNode.Add(attackSequence);
        rootNode.Add(moveSequence);
    }

    void Update()
    {
        if (target != null)
        {
            currentSkill = fuzzy.DecideSkillFuzzy(currentDistance, currentHp);
        }
        else
        {
            currentSkill = SkillAction.Slash;
        }

        rootNode.Evaluate();

        if (currentSkillText != null)
            currentSkillText.text = "현재 스킬: " + currentSkill.ToString();

        if (currentHpText != null)
            currentHpText.text = "현재 HP: " + currentHp.ToString("F0");
    }

    // ================================
    // 상태 체크 함수들
    // ================================

    INode.State CheckPhase()
    {
        if (isAttackTurn)
        {
            return INode.State.Success;
        }
        else
        {
            return INode.State.Failed;
        }
    }

    INode.State CheckAttackRange()
    {
        currentDistance = Vector2.Distance(transform.position, target.position);
        return INode.State.Success;
    }

    INode.State IsAttacking()
    {
        if (IsAnimationRunning) return INode.State.Run;
        return INode.State.Success;
    }

    INode.State IsMoving()
    {
        if (IsAnimationRunning) return INode.State.Failed;

        return INode.State.Success;
    }

    // ================================
    // 행동 함수들
    // ================================

    INode.State attackSelector_Evaluate()
    {
        switch (currentSkill)
        {
            case SkillAction.Slash: return Slash();
            case SkillAction.Shot: return Shot();
            case SkillAction.AreaAttack: return AreaAttack();
            case SkillAction.JumpSmash: return JumpSmash();
            default: return INode.State.Failed;
        }
    }

    INode.State move()
    {
        Debug.Log("Move 시작!");

        if (!IsAnimationRunning)
        {
            IsAnimationRunning = true;

            StartCoroutine(Coroutine());
        }

        Vector2 direction = (target.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);

        return INode.State.Run;
    }

    INode.State Slash()
    {
        if (!IsAnimationRunning)
        {
            IsAnimationRunning = true;
            Debug.Log("Slash 공격 시작!");
            StartCoroutine(Coroutine());
        }
        return INode.State.Run;
    }

    INode.State Shot()
    {
        if (!IsAnimationRunning)
        {
            IsAnimationRunning = true;
            Debug.Log("Shot 공격 시작!");
            StartCoroutine(Coroutine());
        }
        return INode.State.Run;
    }

    INode.State AreaAttack()
    {
        if (!IsAnimationRunning)
        {
            IsAnimationRunning = true;
            Debug.Log("AreaAttack 공격 시작!");
            StartCoroutine(Coroutine());
            
        }
        return INode.State.Run;
    }

    INode.State JumpSmash()
    {
        if (!IsAnimationRunning)
        {
            IsAnimationRunning = true;
            Debug.Log("JumpSmash 공격 시작!");
            StartCoroutine(Coroutine());
        }
        return INode.State.Run;
    }

    // ================================
    // 코루틴 (공격/이동 후 처리)
    // ================================

    IEnumerator Coroutine()
    {
        Debug.Log("실행 중...");
        yield return new WaitForSeconds(2f);
        IsAnimationRunning = false;
        isAttackTurn = !isAttackTurn;
    }
}
