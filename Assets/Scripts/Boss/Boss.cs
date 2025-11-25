using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
public class Boss : EnemyBase
{
    [Header("Boss Only Settings")]
    [SerializeField] private float enragedThreshold = 0.3f;
    [SerializeField] private EnemyAttackPattern[] bossPatternsPhase1;
    [SerializeField] private EnemyAttackPattern[] bossPatternsPhase2;


    public override void Init()
    {
        InitData();
        InitSharedComponents();
        SetState();
        
        StartCoroutine(SpawnRoutine());
        animController = GetComponentInChildren<BossAnimatorController>();
    }

    private IEnumerator SpawnRoutine()
    {
        // Spawn 애니메이터 설정
        if (animController is BossAnimatorController anim)
        {
            SetCurrentAnimator();
            SetAttackPattern();
            anim.PlaySpawn();
            yield return new WaitForSeconds(1);
            // Spawn 애니메이션 길이만큼 대기 
            yield return new WaitForSeconds(anim.GetAnimator().GetCurrentAnimatorClipInfo(0)[0].clip.length);
            anim.PlayStartBattle();
            UIManager.Instance.bossUI.SetBossName(data.Name);
            UIManager.Instance.bossUI.SetActiveBossUI(true);
            UIManager.Instance.bossUI.SetBossHealth(_currentHealth, data.maxHealth);
            yield return new WaitForSeconds(1);

            StartCoroutine(SetShaderMainTextureAfterFirstFrame());

            // 상태 머신 시작
            StateMachine.ChangeState<ChaseState>();
        }
    }
    public override void SetCurrentAnimator()
    {
        animController.SetAnimator(data.animators[0]);
        animController.PlayChase();
    }
    protected override void SetState()
    {
        StateMachine = new StateMachine<EnemyState>();

        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackState(this));
        StateMachine.AddState(new ParriedState(this));
        StateMachine.AddState(new DamagedState(this));
        StateMachine.AddState(new DeadState(this));

        // Spawn 중에는 상태 변경 금지하도록 초기 상태 없음
    }

    protected override void OnDamaged()
    {
        base.OnDamaged();
        SetAttackPattern();

        UIManager.Instance.bossUI.SetBossHealth(_currentHealth, data.maxHealth);
    }

    private void SetAttackPattern()
    {
        if (data is BossData d && animController is BossAnimatorController anim)
        {
            int attackIndex = d.GetPatternLength() - _currentHealth;
            d.SetAttackPattern(d.GetEnemyAttackPatternAtIndex(d.GetPatternLength() - _currentHealth));
            anim.SetAttackIndex(attackIndex);

        }
    }

    
}
