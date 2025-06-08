using UnityEngine;
using UnityEngine.UI;
public enum EBossSkillAction
{
    Slash,         // 근접 공격
    Shot,          // 원거리 공격
    AreaAttack,    // 광역 공격
    JumpSmash      // 점프 후 강타
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
        StateMachine.AddState(new BossDamaged(this)); // DamagedState는 공용 상태를 사용한다고 가정
 
            bossAnim.PlaySpawn();
        
        Invoke("StartFSM",5f);
    }


    public void StartFSM()
    {
        StateMachine.ChangeState<BossIdle>();
    
        bossAnim.PlayStartBattle();
        
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
        // EBossSkillAction skillType = fuzzy.DecideSkill(Vector2.Distance(transform.position, PlayerScript.Instance.GetPlayerTransform().position),
        // data.currentHealth, PlayerScript.Instance.Health, PlayerScript.Instance.GetRigidbody().linearVelocity.magnitude);

        // bossData.AttackPatternSet((int)skillType);
        bossData.SetrandomAttackPattern();
    }

    // 보스 전용 Getter
    public bool CheckPostAttackPauseComplete(float timer) => timer >= bossData.postAttackPauseTime;

}