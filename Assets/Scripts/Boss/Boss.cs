using UnityEngine;
using UnityEngine.UI;
public enum EBossSkillAction
{
    Rush = 0,
    Meteo = 1,
    MAX

}
public class Boss : EnemyBase
{

    // UI (보스만 가질 수 있음)
    public Text StateText;
    public Text AttackText;

    BossAnimatorController bossAnim; // 보스 전용 애니메이터 컨트롤러
    private BossData bossData; // 보스 전용 데이터
    private Fuzzy fuzzy;
    private float _skillCooldownTimer = 0f;

    /// <summary>
    /// 보스 초기화. 부모의 Init을 호출하고 보스만의 로직을 추가합니다.
    /// </summary>
    public override void Init()
    {
        // 보스 전용 캐스팅
        bossAnim = animController as BossAnimatorController;
        bossData = data as BossData;

        base.Init();

        // 보스 전용 초기화
        fuzzy = new Fuzzy();
        _skillCooldownTimer = 0f;

        GetComponent<CircleCollider2D>().enabled = false; // 보스의 충돌체 비 활성화
    }

    /// <summary>
    /// 보스의 상태 머신을 설정합니다.
    /// </summary>
    protected override void SetState()
    {
        StateMachine = new StateMachine<IEnemyState>();

        // 보스 전용 상태들 등록
        StateMachine.AddState(new BossIdle(this));
        StateMachine.AddState(new BossMove(this));
        StateMachine.AddState(new BossSkillAttack(this));
        StateMachine.AddState(new BossCooldown(this));
        StateMachine.AddState(new BossDead(this));


        bossAnim.PlaySpawn();

        Invoke("StartBattle", 5f); // 나중에 애니메이션 종료타이밍과 연동
    }


    public void StartBattle()
    {
        GetComponent<CircleCollider2D>().enabled = true;
        UIManager.Instance.bossUI.SetBossMaxHealth(bossData.maxHealth);
        UIManager.Instance.bossUI.SetBossName(bossData.Name);
        UIManager.Instance.bossUI.SetActiveBossUI(true);
        bossAnim.PlayStartBattle();
        StateMachine.ChangeState<BossIdle>();


    }

    protected override void OnDamaged()
    {
        UIManager.Instance.bossUI.SetBossHealth(bossData.currentHealth);
        GetAnimatorController().PlayDamage();
    }
    protected override void Dead()
    {
        isDead = true;
        UIManager.Instance.bossUI.SetBossHealth(bossData.currentHealth);
        StateMachine.ChangeState<BossDead>();


    }
    // 스킬 쿨타임 관리 로직
    public void UpdateSkillCooldown()
    {


        _skillCooldownTimer += Time.deltaTime;
        if (_skillCooldownTimer >= bossData.attackCooldown)
        {
            StateMachine.ChangeState<BossSkillAttack>();
            _skillCooldownTimer = 0f;
        }
    }


    // 퍼지 로직으로 스킬 결정
    public void DecideSkill()
    {
        float distance = Vector2.Distance(transform.position, PlayerScript.Instance.GetPlayerTransform().position);
        float bossHp = data.currentHealth;
        float playerHp = PlayerScript.Instance.Health;
        float playerSpeed = PlayerScript.Instance.GetRigidbody().linearVelocity.magnitude;

    // 퍼지 추론으로 스킬 결정
        int predictedSkillIndex = fuzzy.PredictSkill(bossHp, playerHp, distance, playerSpeed);

    // Enum으로 변환
    if (predictedSkillIndex >= (int)EBossSkillAction.MAX || predictedSkillIndex < 0)
    {
        Debug.LogWarning("Fuzzy 추론 결과가 유효하지 않습니다. 기본 스킬로 설정.");
        predictedSkillIndex = 0;
    }

    EBossSkillAction skillType = (EBossSkillAction)predictedSkillIndex;

    Debug.Log($"사용한 스킬: {skillType}");

    bossAnim.SetAttackIndex(predictedSkillIndex);
    bossData.AttackPatternSet(predictedSkillIndex);

    }
    public override void Parried()
    {

    }
    // 보스 전용 Getter
    public bool CheckPostAttackPauseComplete(float timer) => timer >= bossData.postAttackPauseTime;

}