using UnityEngine;
using System.Collections;
public class Boss : EnemyBase
{
    [Header("Boss Only Settings")]
    [SerializeField] private float enragedThreshold = 0.3f;
    [SerializeField] private EnemyAttackPattern[] bossPatternsPhase1;
    [SerializeField] private EnemyAttackPattern[] bossPatternsPhase2;

    private bool isEnraged = false;

    public override void Init()
    {
        InitData();
        InitSharedComponents();
        SetState();
        // Spawn 애니메이션 재생 (아직 행동 금지)
        StartCoroutine(SpawnRoutine());
        animController = GetComponentInChildren<BossAnimatorController>();
    }

    private IEnumerator SpawnRoutine()
    {
        // Spawn 애니메이터 설정
        if (data is BossData bossData && animController is BossAnimatorController anim)
        {
            anim.SetAnimator(bossData.spawnAnimator);
            anim.PlaySpawn();
        }

        // Spawn 애니메이션 길이만큼 대기 (약 2초, 필요시 조정)
        yield return new WaitForSeconds(2f);

        // Spawn 애니메이션 종료 후 일반 애니메이터로 변경
        SetCurrentAnimator();
        StartCoroutine(SetShaderMainTextureAfterFirstFrame());

        // 상태 머신 시작
        StateMachine.ChangeState<IdleState>();
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
    }

    public EnemyAttackPattern GetCurrentPattern()
    {
        if (isEnraged) return bossPatternsPhase2[Random.Range(0, bossPatternsPhase2.Length)];
        return bossPatternsPhase1[Random.Range(0, bossPatternsPhase1.Length)];
    }
}
