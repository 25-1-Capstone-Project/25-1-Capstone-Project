using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Core Components & Data")]
    [SerializeField] protected ParticleSystem stunEffect;
    public ParticleSystem GetStunEffect() => stunEffect;
    [SerializeField] protected EnemyDataBase data; // 모든 적은 데이터를 가짐
    [SerializeField] protected SpriteRenderer enemySprite;
    [SerializeField] protected EnemyAnimatorController animController;
    protected AIAgent aIAgent;
    public EnemyAttackPattern GetAttackPattern() => data.attackPattern;
    public int GetDamage() => data.attackDamage;
    protected Rigidbody2D rb;
    [SerializeField] public EnemyShaderController enemyShaderController; // 적 스크립트 (Enemy, Boss 등)
    protected EnemyDataBase Data => data; // 외부에서 데이터 접근을 위한 프로퍼티
    // State Machine
    public StateMachine<EnemyState> StateMachine { get; protected set; }

    // Common States
    public bool IsAttacking;// 공격 중인지 여부 (State에서 제어)
    protected bool isDead = false;
    public bool IsStunned;
    public bool CheckStunned() => IsStunned;

    public int _stamina;
    public int Stamina
    {
        get { return _stamina; }
        set
        {
            _stamina = Mathf.Max(0, value);
            if (_stamina == 0)
            {
                OnParried();
            }
        }
    }
    // Health Property
    protected int _currentHealth;
    public int Health
    {
        get { return _currentHealth; }
        protected set
        {
            if (isDead) return; // 이미 죽었다면 체력 변경 방지
            
            _currentHealth = Mathf.Max(0, value);

            if (_currentHealth == 0)
            {
                Dead();
            }
            else
            {
                // 피격 상태로 전환 (이 부분은 자식 클래스에서 다르게 처리할 수 있음)
                OnDamaged();
            }
        }
    }

    #region Unity Lifecycle

    protected virtual void Start()
    {
        Init();

    }

    protected virtual void Update()
    {
        StateMachine?.Update();
    }

    protected virtual void LateUpdate()
    {
        StateMachine?.LateUpdate();
    }

    protected virtual void FixedUpdate()
    {
        StateMachine?.FixedUpdate();
    }

    protected virtual void OnDestroy()
    {
        ClearAttackEffect();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// 적 개체를 초기화합니다. 자식 클래스에서 이 메서드를 오버라이드하여
    /// 자신만의 초기화 로직을 추가할 수 있습니다.
    /// </summary>
    public virtual void Init()
    {
        InitData();
        InitSharedComponents();
        SetState();
        SetCurrentAnimator();
        StartCoroutine(SetShaderMainTextureAfterFirstFrame());

    }

    public IEnumerator OutLineRoutine(float time)
    {
        yield return new WaitForSeconds(time - 0.2f);
        enemyShaderController.OnOutline();
        yield return new WaitForSeconds(0.2f);
    }
    /// <summary>
    /// 모든 적이 공통으로 사용하는 컴포넌트를 초기화합니다.
    /// </summary>
    protected void InitSharedComponents()
    {
        stunEffect.gameObject.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        aIAgent = GetComponent<AIAgent>();
        aIAgent.SetSpeed(data.moveSpeed);
    }
    public virtual void SetCurrentAnimator()
    {
        int index = data.animators.Length - _currentHealth;
        animController.SetAnimator(data.GetAnimatorAtIndex(index));
        animController.PlayIdle();
    }
    protected IEnumerator SetShaderMainTextureAfterFirstFrame()
    {
        yield return new WaitForEndOfFrame();
        enemyShaderController.InitMaterial();
    }
    /// <summary>
    /// </summary>
    protected void InitData()
    {
        if (data == null)
        {
            Debug.LogError($"{gameObject.name}에 EnemyBaseData가 할당되지 않았습니다.");
            return;
        }
        _currentHealth = data.maxHealth;
        _stamina = data.stamina;
        isDead = false;

    }
    public void InitStamina()
    {

        _stamina = data.stamina;
    }
    /// <summary>
    /// 이 적의 상태 머신을 설정합니다.
    /// 자식 클래스(Enemy, Boss)에서 반드시 구현해야 합니다.
    /// </summary>
    protected virtual void SetState()
    {
        StateMachine = new StateMachine<EnemyState>();

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

    #endregion

    #region Common Actions & Behaviours

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        AudioManager.Instance.PlaySFX("Damaged");
        Health -= damage;
        FlashSprite(Color.red, 0.1f);
        //hpBar?.SetHealth(_currentHealth, data.maxHealth);
    }
    protected virtual void OnParried()
    {
        AudioManager.Instance.PlaySFX("ParrySuccess");
        StateMachine.ChangeState<ParriedState>();
    }
    public void SetStunEffectActive(bool isActive)
    {

        stunEffect.gameObject.SetActive(isActive);

    }
    /// <summary>
    /// 피격 시 호출되는 메서드. 자식 클래스에서 오버라이드
    /// </summary>
    protected virtual void OnDamaged()
    {

        StateMachine.ChangeState<DamagedState>();
    }

    protected virtual void Dead()
    {

        isDead = true;
        StateMachine.ChangeState<DeadState>();
        PlayerLogger.Instance.PlusEnemyKilledLog(); // 적 처치 기록
                                                    // hpBar?.Hide();
    }

    public void KnockBack(float knockBackForce)
    {
        //animController.PlayKnockBack();
        rb.linearVelocity = -GetDirectionNormalVec() * knockBackForce;

    }

    public void FlashSprite(Color color, float duration)
    {
        if (enemySprite == null) return;
        StartCoroutine(FlashRoutine(color, duration));
    }


    public void SpriteFlip()
    {
        enemySprite.flipX = GetDirectionNormalVec().x < 0;

    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        enemySprite.color = color;
        yield return new WaitForSeconds(duration);
        enemySprite.color = Color.white;
    }

    #endregion



    // 공격 예고선 관련 로직은 공통으로 사용될 수 있음
    public LineRenderer CurrentSpearIndicator { get; set; }
    public void ClearAttackEffect()
    {
        if (CurrentSpearIndicator != null)
        {
            CurrentSpearIndicator.gameObject.SetActive(false);
            CurrentSpearIndicator = null;
        }
    }


    public virtual bool CheckAttackRange()
    {

        if (data.attackPattern == null) return false;
        if (data.attackPattern.attackRange == -1) return true; // 사거리 무제한 처리
        return GetDirectionToPlayerVec().magnitude < data.attackPattern.attackRange;
    }


    #region Getters & Setters

    public Rigidbody2D GetRigidbody() => rb;
    public AIAgent GetAIAgent() => aIAgent;
    public float GetSpeed() => data.moveSpeed;
    public EnemyAnimatorController GetAnimatorController() => animController;
    public EnemyDataBase GetData() => data;

    public Vector2 GetDirectionToPlayerVec() => GameManager.Instance.playerScript.GetPlayerTransform().position - transform.position;
    public Vector2 GetDirectionNormalVec() => GetDirectionToPlayerVec().normalized;

    public void SetEnemyData(EnemyDataBase newData) => data = newData;

    #endregion
}