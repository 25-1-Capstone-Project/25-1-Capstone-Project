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
    float currentDistance;

    Rigidbody2D rb;
    Vector2 dir;
    SelectorNode rootNode;
    SelectorNode attackSelector;
    INode.State state = INode.State.Success;
    SequenceNode attackSequence;
    SequenceNode moveSequence;
    Fuzzy fuzzy = new Fuzzy();
    SkillAction currentSkill = SkillAction.Slash;

    Vector2 originPos;

    float currentHp = 100f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        originPos = transform.position;

        attackSequence = new SequenceNode(); // 공격 시퀀스 노드 생성
        attackSequence.Add(new ActionNode(Cooldown)); // 쿨타임 체크 노드 추가
        attackSequence.Add(new ActionNode(CheckAttackRange)); // 거리 체크 노드 추가
        attackSequence.Add(new ActionNode(IsAttacking));
        attackSequence.Add(new ActionNode(attackSelector_Evaluate));

        moveSequence = new SequenceNode(); // 이동 시퀀스 노드 생성
        moveSequence.Add(new ActionNode(IsMoving)); // 이동 체크 노드 추가
        moveSequence.Add(new ActionNode(move)); // 이동 액션

        rootNode = new SelectorNode();
        rootNode.Add(attackSequence); // 루트 노드의 자식으로 공격 시퀀스
        rootNode.Add(moveSequence); // 이동 시퀀스

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

    // 공격 액션 Slash, Shot, AreaAttack, JumpSmash
    INode.State Cooldown()
    {
        return INode.State.Success;
    }
    INode.State CheckAttackRange()
    {
        currentDistance = Vector2.Distance(transform.position, target.position);

        return INode.State.Success;
    }
    INode.State IsAttacking()
    {
        if (IsAnimationRunning)
        {
            return INode.State.Run;
        }

        return INode.State.Success;
    }

    INode.State attackSelector_Evaluate()
    {
        switch(currentSkill)
    {
        case SkillAction.Slash:
            return Slash();
        case SkillAction.Shot:
            return Shot();
        case SkillAction.AreaAttack:
            return AreaAttack();
        case SkillAction.JumpSmash:
            return JumpSmash();
        default:
            return INode.State.Failed;
    }
    }

    INode.State IsMoving()
    {
        if (IsAnimationRunning)
        {
            Coroutine();
            return INode.State.Run;
        }

        return INode.State.Run;
    }
        
    INode.State move()
    {
        Debug.Log("Move");

        if (IsAnimationRunning)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);

            return INode.State.Run;
        }

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

    IEnumerator Coroutine()
    {
        Debug.Log("실행 중...");
        yield return new WaitForSeconds(2f);
        IsAnimationRunning = false;
    }
}