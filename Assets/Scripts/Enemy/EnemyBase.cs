using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Core Components & Data")]
    [SerializeField] protected ParticleSystem stunEffect;
    [SerializeField] protected EnemyDataBase data;
    [SerializeField] protected SpriteRenderer enemySprite;
    [SerializeField] protected EnemyAnimatorController animController;
    protected AIAgent aIAgent;
    [SerializeField] private EnemyHPBar hpBar;
    [SerializeField] public EnemyShaderController enemyShaderController;


    [Header("분리 설정 (Boids Separation)")]
    public float separationRadius = 0.5f;     // 서로 밀어낼 감지 반경 (★중요: 작게 설정)
    public float separationStrength = 0.5f;   // 밀어내는 힘의 강도 (★중요: moveSpeed보다 약하게)
    public float separationUpdateFrequency = 0.2f; // 분리 계산 주기 (0.1 ~ 0.3초)
    public LayerMask enemyLayerMask;            // "Enemy" 레이어 마스크
    // 캐시될 분리 벡터 (★핵심)
    private Vector2 cachedSeparationVector = Vector2.zero;

    // NonAlloc을 위한 캐시 배열 (GC 방지)
    private Collider2D[] neighborBuffer = new Collider2D[30];
    protected Rigidbody2D rb;
    protected Transform playerTransform; 

    // State Machine
    public StateMachine<IEnemyState> StateMachine { get; protected set; }

    // Common States
    public bool IsAttacking { get; set; }
    protected bool isDead = false;

    // 🔧 상태별 코루틴 안전 관리 (StopAllCoroutines 제거)
    private Coroutine runningAction;
    public void RunAction(IEnumerator routine)
    {
        StopAction();
        runningAction = StartCoroutine(routine);
    }
    public void StopAction()
    {
        if (runningAction != null) { StopCoroutine(runningAction); runningAction = null; }
    }

    // Stamina / Health
    public int _stamina;
    public int Stamina
    {
        get => _stamina;
        set
        {
            _stamina = Mathf.Max(0, value);
            if (_stamina == 0) OnParried();
        }
    }

    protected int _currentHealth;
    public int Health
    {
        get => _currentHealth;
        protected set
        {
            if (isDead) return;
            _currentHealth = Mathf.Max(0, value);
            if (_currentHealth == 0) { Dead(); }
            else { OnDamaged(); }
        }
    }

    #region Unity Lifecycle
    protected virtual void Start()
    {
        Init();
    }

    protected virtual void Update() => StateMachine?.Update();
    protected virtual void LateUpdate() => StateMachine?.LateUpdate();
    protected virtual void FixedUpdate() => StateMachine?.FixedUpdate();

    protected virtual void OnDisable() => StopAction(); // 🔧 상태 코루틴 정리
    protected virtual void OnDestroy() => ClearAttackEffect();
    #endregion

    #region Initialization
    public virtual void Init()
    {
        InitSharedComponents();
        InitData();
        SetState();
    }

    private void InitSharedComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        aIAgent = GetComponent<AIAgent>();
        playerTransform = PlayerScript.Instance != null ? PlayerScript.Instance.GetPlayerTransform() : null;

        if (animController != null && data != null)
        {
            animController.SetAnimator(data.animator);
            StartCoroutine(SetShaderMainTextureAfterFirstFrame());
        }
        enemyLayerMask = LayerMask.GetMask("Enemy");
        StartCoroutine(UpdateSeparationVectorCoroutine());
    }

    private IEnumerator SetShaderMainTextureAfterFirstFrame()
    {
        yield return new WaitForEndOfFrame();
        enemyShaderController?.InitMaterial();
    }

    private void InitData()
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

    public void InitStamina() => _stamina = data.stamina;

    protected virtual void SetState()
    {
        StateMachine = new StateMachine<IEnemyState>();
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackState(this));
        StateMachine.AddState(new ParriedState(this));
        StateMachine.AddState(new DamagedState(this));
        StateMachine.AddState(new DeadState(this));
        StateMachine.ChangeState<ChaseState>();
    }
    #endregion

    #region Common Actions & Behaviours
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        AudioManager.Instance?.PlaySFX("AttackHit");
        Health -= damage;
        FlashSprite(Color.red, 0.1f);
        hpBar?.SetHealth(_currentHealth, data.maxHealth);
    }

    protected virtual void OnParried()
    {
        if (!data.dontStopEnemy) stunEffect?.Play();
        StateMachine.ChangeState<ParriedState>();
    }

    protected virtual void OnDamaged()
    {
        StateMachine.ChangeState<DamagedState>();
    }

    protected virtual void Dead()
    {
        stunEffect?.Stop();
        isDead = true;
        StateMachine.ChangeState<DeadState>();
        PlayerLogger.Instance?.PlusEnemyKilledLog();
        hpBar?.Hide();
    }

    public void KnockBack(float knockBackForce)
    {
        animController?.PlayKnockBack();

        rb.linearVelocity = -GetDirectionNormalVec() * knockBackForce;
    }

    public void FlashSprite(Color color, float duration)
    {
        if (!enemySprite) return;
        StartCoroutine(FlashRoutine(color, duration));
    }

    public void SpriteFlip()
    {
        if (!playerTransform) return;
        enemySprite.flipX = GetDirectionNormalVec().x < 0;
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        var prev = enemySprite.color;
        enemySprite.color = color;
        yield return new WaitForSeconds(duration);
        enemySprite.color = prev;
    }
    #endregion


    public void Move()
    {
        if (playerTransform == null) return;

        // 1. 플레이어 방향 벡터
        Vector2 moveTowardsPlayer = (playerTransform.position - transform.position).normalized;

        // 2. 최종 이동 벡터 = 플레이어 방향 + 캐시된 분리 방향
        // cachedSeparationVector는 평소엔 (0,0)이다가, 겹칠 때만 값이 생깁니다.
        Vector2 finalMoveVector = (moveTowardsPlayer + cachedSeparationVector).normalized;

        // 3. MovePosition으로 이동
        rb.MovePosition(rb.position + finalMoveVector * data.moveSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator UpdateSeparationVectorCoroutine()
    {
        // 게임이 끝날 때까지 무한 반복
        while (true)
        {
            // 계산 실행
            CalculateSeparation();

            // ★ 핵심: 다음 계산까지 대기
            yield return new WaitForSeconds(separationUpdateFrequency);
        }
    }
    private void CalculateSeparation()
    {
        // 1. 주변(separationRadius)의 모든 'Enemy' 레이어 콜라이더 탐지
        int count = Physics2D.OverlapCircleNonAlloc(rb.position, separationRadius, neighborBuffer, enemyLayerMask);

        if (count <= 1) // 1 이하는 '나 자신' 뿐이라는 의미
        {
            cachedSeparationVector = Vector2.zero;
            return; // 주변에 아무도 없으면 계산 종료
        }

        // 2. 분리 벡터 계산
        Vector2 separationSum = Vector2.zero;
        int neighborsFound = 0;

        for (int i = 0; i < count; i++)
        {
            Collider2D neighbor = neighborBuffer[i];

            // 나 자신은 제외
            if (neighbor.attachedRigidbody == rb) continue;

            // 나와 이웃 사이의 방향 벡터 (나 -> 이웃)
            Vector2 directionToNeighbor = (Vector2)neighbor.transform.position - rb.position;

            // 나와 이웃 사이의 거리 (제곱근 계산은 비싸므로 sqrMagnitude 사용)
            float sqrDistance = directionToNeighbor.sqrMagnitude;

            // 너무 가깝다면 (0.0001f는 0으로 나누기 방지)
            if (sqrDistance < separationRadius * separationRadius)
            {
                // ★ 방향 벡터: 이웃으로부터 "멀어지는" 방향
                Vector2 awayFromNeighbor = -directionToNeighbor;

                // ★ 가중치: 가까울수록 더 강하게 밀도록 (거리의 역수에 비례)
                separationSum += awayFromNeighbor.normalized / (sqrDistance + 0.0001f);
                neighborsFound++;
            }
        }

        if (neighborsFound > 0)
        {
            // 3. 계산된 평균 분리 벡터를 캐시에 저장
            cachedSeparationVector = (separationSum / neighborsFound).normalized * separationStrength;
        }
        else
        {
            cachedSeparationVector = Vector2.zero;
        }
    }
    // 공격 예고선
    public LineRenderer CurrentSpearIndicator { get; set; }
    public void ClearAttackEffect()
    {
        if (CurrentSpearIndicator)
        {
            CurrentSpearIndicator.gameObject.SetActive(false);
            CurrentSpearIndicator = null;
        }
    }

    public virtual bool CheckAttackRange()
    {
        if (data?.attackPattern == null || !playerTransform) return false;
        return GetDirectionToPlayerVec().sqrMagnitude < data.attackPattern.attackRange * data.attackPattern.attackRange;
    }

    #region Getters & Setters
    public Rigidbody2D GetRigidbody() => rb;
    public AIAgent GetAIAgent() => aIAgent;
    public float GetSpeed() => data.moveSpeed;
    public EnemyAnimatorController GetAnimatorController() => animController;
    public EnemyDataBase GetData() => data;

    public Vector2 GetDirectionToPlayerVec()
        => playerTransform ? (Vector2)playerTransform.position - (Vector2)transform.position : Vector2.zero;

    public Vector2 GetDirectionNormalVec()
    {
        var v = GetDirectionToPlayerVec();
        return v.sqrMagnitude > 0.0001f ? v.normalized : Vector2.zero;
    }

    public void SetEnemyData(EnemyDataBase newData) => data = newData;
    #endregion
}
