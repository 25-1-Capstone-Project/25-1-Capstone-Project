using UnityEngine;

public class NormalEnemy : EnemyBase
{
    [SerializeField] private EnemyHPBar hpBar;
    public GameObject skillSelectItemPrefab;
    // EnemyBase에 Init()이 있으므로, 추가적인 초기화가 필요 없다면 override하지 않아도 됨.
    // public override void Init()
    // {
    //     base.Init(); // 부모의 Init() 호출
    //     // 일반 적만의 추가 초기화 로직
    // }

    /// <summary>
    /// 일반 적의 상태 머신을 설정합니다.
    /// </summary>
    protected override void SetState()
    {
        StateMachine = new StateMachine<IEnemyState>();

        // 일반 적을 위한 상태들 등록
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackState(this));
        StateMachine.AddState(new ParriedState(this));
        StateMachine.AddState(new DamagedState(this));
        StateMachine.AddState(new DeadState(this));

        // 초기 상태 설정
        StateMachine.ChangeState<IdleState>();
    }

    // OnDamaged, Dead 등 부모의 virtual 메서드를 필요에 따라 오버라이드할 수 있습니다.
    // 예를 들어, 일반 적은 피격 시 무조건 넉백된다면:
    protected override void OnDamaged()
    {

        base.OnDamaged(); // KnockBackState가 없다면 부모의 기본 행동 수행

    }
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        hpBar?.SetHealth(data.currentHealth, data.maxHealth);
    }
    protected override void Dead()
    {
        base.Dead();
        hpBar?.Hide();
    }
}