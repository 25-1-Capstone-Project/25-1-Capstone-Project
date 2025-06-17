using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Core Components & Data")]
    [SerializeField] protected EnemyBaseData data; // 모든 적은 데이터를 가짐
    [SerializeField] protected SpriteRenderer enemySprite;
    [SerializeField] protected EnemyAnimatorController animController;

    public EnemyAttackPattern GetAttackPattern() => data.attackPattern;
    public int GetDamage() => data.attackDamage;
    protected Rigidbody2D rb;
    [SerializeField] public EnemyShaderController enemyShaderController; // 적 스크립트 (Enemy, Boss 등)
    protected EnemyBaseData Data => data; // 외부에서 데이터 접근을 위한 프로퍼티
    // State Machine
    public StateMachine<IEnemyState> StateMachine { get; protected set; }

    // Common States
    public bool IsAttacking { get; set; } // 공격 중인지 여부 (State에서 제어)
    protected bool isDead = false;

    // Health Property
    public int Health
    {
        get { return data.currentHealth; }
        protected set
        {
            if (isDead) return; // 이미 죽었다면 체력 변경 방지

            data.currentHealth = Mathf.Max(0, value);

            if (data.currentHealth <= 0)
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
    /// 자신만의 초기화 로직(예: 퍼지 로직)을 추가할 수 있습니다.
    /// </summary>
    public virtual void Init()
    {
        InitSharedComponents();
        InitData();
        SetState();
    }

    /// <summary>
    /// 모든 적이 공통으로 사용하는 컴포넌트를 초기화합니다.
    /// </summary>
    private void InitSharedComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        // null 체크를 추가하여 안정성 확보
        if (animController != null && data != null)
        {
            animController.SetAnimator(data.animator);
        }
    }

    /// <summary>
    /// ScriptableObject 데이터 관련 초기화를 수행합니다.
    /// </summary>
    private void InitData()
    {
        if (data == null)
        {
            Debug.LogError($"{gameObject.name}에 EnemyBaseData가 할당되지 않았습니다.");
            return;
        }
        data.currentHealth = data.maxHealth;
        isDead = false;
    }

    /// <summary>
    /// 이 적의 상태 머신을 설정합니다.
    /// 자식 클래스(Enemy, Boss)에서 반드시 구현해야 합니다.
    /// </summary>
    protected abstract void SetState();

    #endregion

    #region Common Actions & Behaviours

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        AudioManager.Instance.PlaySFX("AttackHit"); // 오디오 매니저가 있다면
        Health -= damage;
        FlashSprite(Color.red, 0.1f);

    }

    /// <summary>
    /// 피격 시 호출되는 메서드. 자식 클래스에서 오버라이드
    /// </summary>
    protected virtual void OnDamaged()
    {
        if (StateMachine.GetCurrentState() is AttackState)
        {
            
            return;
        }
        StateMachine.ChangeState<DamagedState>();
    }
    public virtual void Parried()
    {
        StateMachine.ChangeState<ParriedState>();
    }
    protected virtual void Dead()
    {
        isDead = true;
        StateMachine.ChangeState<DeadState>();
    }

    public void KnockBack(float knockBackForce)
    {
        animController.PlayKnockBack();
        rb.linearVelocity = -GetDirectionToPlayerNormalVec() * knockBackForce;
    }

    public void FlashSprite(Color color, float duration)
    {
        if (enemySprite == null) return;
        StartCoroutine(FlashRoutine(color, duration));
    }
   

    public void SpriteFlip()
    {
        enemySprite.flipX = GetDirectionToPlayerNormalVec().x < 0;

    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        enemySprite.color = color;
        yield return new WaitForSeconds(duration);
        enemySprite.color = Color.white;
    }

    #endregion

    #region Attack Logic (자식 클래스에서 구체화)

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

    /// <summary>
    /// 현재 설정된 공격 패턴을 실행합니다.
    /// 이 메서드는 State 클래스(AttackState, BossAttackState 등)에서 호출됩니다.
    /// </summary>
    public virtual void ExecuteCurrentAttack()
    {
        if (data.attackPattern == null) return;
        StartCoroutine(data.attackPattern.Execute(this));
    }

    public virtual bool CheckAttackRange()
    {
        if (data.attackPattern == null) return false;
        return GetDirectionToPlayerVec().magnitude < data.attackPattern.attackRange;
    }

    #endregion

    #region Getters & Setters

    public Rigidbody2D GetRigidbody() => rb;
    public float GetSpeed() => data.moveSpeed;
    public EnemyAnimatorController GetAnimatorController() => animController;
    public EnemyBaseData GetData() => data;

    public Vector2 GetDirectionToPlayerVec() => PlayerScript.Instance.GetPlayerTransform().position - transform.position;
    public Vector2 GetDirectionToPlayerNormalVec() => GetDirectionToPlayerVec().normalized;

    public void SetEnemyData(EnemyBaseData newData) => this.data = newData;

    #endregion
}