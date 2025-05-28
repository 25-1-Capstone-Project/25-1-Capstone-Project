using System.Collections;
using UnityEditor;
using UnityEngine;


public class Enemy : MonoBehaviour
{

    [SerializeField] EnemyData enemyData;
    [SerializeField] SpriteRenderer enemySprite;
    [SerializeField] EnemyAnimatorController enemyAnimController;
    [SerializeField] EnemyAttackPattern attackPattern;
    [SerializeField] ParticleSystem attackParticle;
    private Rigidbody2D rb;

    // StateMachine Property
    public StateMachine<EnemyState> StateMachine { get; private set; }
    [SerializeField] private float flashDuration = 0.1f;
    public bool IsAttacking;
    private bool isDead;
    private float speed;

    int health;
    int Health
    {
        get { return health; }
        set
        {
            health = value;

            if (health <= 0)
            {
                health = 0;
                Dead();
            }
        }
    }
    public void Dead()
    {
        isDead = true;
        StateMachine.ChangeState<DeadState>();
    }

    #region 적 공격 예고선
    public LineRenderer CurrentSpearIndicator { get; set; }
    public void ClearAttackEffect()
    {
        if (CurrentSpearIndicator != null)
        {
            CurrentSpearIndicator.gameObject.SetActive(false);
            CurrentSpearIndicator = null;
        }
    }
    #endregion

    #region GetFunction
    public EnemyAttackPattern GetAttackPattern() => attackPattern;
    public int GetDamage() => enemyData.attackDamage;
    public Rigidbody2D GetRigidbody() => rb;
    public float GetSpeed() => speed;
    public EnemyAnimatorController GetAnimatorController() => enemyAnimController;
    public Transform GetPlayer() => PlayerScript.Instance.GetPlayerTransform();

    public Vector2 GetDirectionToPlayerVec() => PlayerScript.Instance.GetPlayerTransform().position - transform.position;
    public Vector2 GetDirectionToPlayerNormalVec() => GetDirectionToPlayerVec().normalized;
    #endregion

    #region SetFunction
    public void SetEnemyData(EnemyData enemyData) => this.enemyData = enemyData;
    #endregion
    #region Initialize
    public void EnemyInit()
    {
        enemyData.AttackPatternSet();
        attackPattern = enemyData.attackPattern;
        enemyAnimController.SetAnimator(enemyData.animator);
        speed = enemyData.moveSpeed;
        health = enemyData.maxHealth;
       
    }
    void SetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void SetStateMachine()
    {
        // Initialize StateMachine
        StateMachine = new StateMachine<EnemyState>();
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackState(this));
        StateMachine.AddState(new ParryState(this));
        StateMachine.AddState(new DamagedState(this));
        StateMachine.AddState(new DeadState(this));

        // Set initial state
        StateMachine.ChangeState<IdleState>();
    }
    #endregion

    #region Unity LifeCycle
    void Start()
    {
        SetComponents();
        SetStateMachine();
    }

    void Update()
    {
        StateMachine.Update();
    }
    void LateUpdate()
    {
        StateMachine.LateUpdate();
    }
    void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }
    void OnDestroy()
    {

        ClearAttackEffect();
    }
    #endregion


    public void SpriteFlip()
    {
        enemySprite.flipX = rb.linearVelocityX == 0 ? enemySprite.flipX : rb.linearVelocityX > 0;

    }
    public bool CheckAttackRange()
    {
        // 플레이어가 가까우면 공격 상태로 전환
        if (GetDirectionToPlayerVec().magnitude < enemyData.attackRange)
            return true;
        else
            return false;
    }
    public void Attack()
    {
        StartCoroutine(attackPattern.Execute(this));
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (!IsAttacking)
            StateMachine.ChangeState<DamagedState>();

        FlashSprite(Color.red, 0.5f);
        Health -= damage;
        Debug.Log("적 아야");
    }
    public void KnockBack(float knockBackForce)
    {
        enemyAnimController.PlayKnockBack();
        rb.linearVelocity = -GetDirectionToPlayerNormalVec() * knockBackForce;
    }

    public void FlashSprite(Color color, float duration)
    {
        StartCoroutine(FlashRoutine(color, duration));
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        enemySprite.color = color;
        yield return new WaitForSeconds(duration);
        enemySprite.color = Color.white;
    }
    // #if UNITY_EDITOR
    //     private void OnDrawGizmos()
    //     {
    //         switch (enemyData.eEnemyType)
    //         {
    //             case EEnemyType.Sword:
    //                 Gizmos.color = Color.blue;
    //                 Vector2 attackDir = GetDirectionNormalVec();
    //                 float attackAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

    //                 Vector2 boxCenter = (Vector2)transform.position + attackDir * 0.5f;
    //                 Vector2 boxSize = new Vector2(1f, 1f); // 혹은 sword.range 등에서 계산

    //                 Gizmos.color = Color.red;

    //                 // 회전 매트릭스로 회전 적용
    //                 Matrix4x4 rot = Matrix4x4.TRS(boxCenter, Quaternion.Euler(0, 0, attackAngle), Vector3.one);
    //                 Gizmos.matrix = rot;
    //                 Gizmos.DrawWireCube(Vector2.zero, new Vector2(boxSize.x, boxSize.y));

    //                 break;
    //             case EEnemyType.Spear:
    //                 Gizmos.DrawSphere(transform.position, 0.5f);

    //                 break;
    //         }


    //     }
    // #endif

}