using System.Collections;
using UnityEngine;


public class Enemy : MonoBehaviour
{

    [SerializeField] EnemyData enemyData;
    [SerializeField] SpriteRenderer enemySprite;
    [SerializeField] EnemyAnimatorController enemyAnimController;
    [SerializeField] EnemyAttackPattern attackPattern;
    [SerializeField] ParticleSystem attackParticle;
    [SerializeField] Transform attackParticleTransform;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    #region GetFunction
    public EnemyAttackPattern GetAttackPattern() => attackPattern;
    public int GetDamage() => enemyData.attackDamage;
    public Rigidbody2D GetRigidbody() => rb;
    public float GetSpeed() => speed;
    public EnemyAnimatorController GetAnimatorController() => enemyAnimController;
    public Transform GetPlayer() => PlayerScript.Instance.GetPlayerTransform();
    public Transform GetAttackParticleT() => attackParticleTransform;
    public ParticleSystem GetAttackParticle() => attackParticle;
    public Vector2 GetDirectionVec() => PlayerScript.Instance.GetPlayerTransform().position - transform.position;
    public Vector2 GetDirectionNormalVec() => GetDirectionVec().normalized;
    
  
    #endregion

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
 
    private Rigidbody2D rb;

    // StateMachine Property
    public StateMachine<EnemyState> StateMachine { get; private set; }

    void Start()
    {
        SetComponents();
        EnemyInit();
        SetStateMachine();
    }
    void EnemyInit()
    {
        speed = enemyData.moveSpeed;
        health = enemyData.maxHealth;

        switch (enemyData.eEnemyType)
        {
            case EEnemyType.Sowrd:
                attackPattern = EnemyManager.Instance.commonEnemyAttackPatterns[0];
                break;
        }
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

    public void SpriteFlip()
    {
        enemySprite.flipX = rb.linearVelocityX == 0 ? enemySprite.flipX : rb.linearVelocityX > 0;

    }
    public void Attack()
    {
        StartCoroutine(attackPattern.Execute(this));
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        StateMachine.ChangeState<DamagedState>();
        FlashOnDamage();
        Health -= damage;
        Debug.Log("적 아야");
    }
    public void KnockBack(float knockBackForce)
    {
        enemyAnimController.PlayKnockBack();
        rb.linearVelocity = -GetDirectionNormalVec() * knockBackForce;
    }
   
    public void FlashOnDamage()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        enemySprite.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        enemySprite.color = Color.white;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!(attackPattern is Enmey_SwordSlash sword)) return;

        Vector2 attackDir = GetDirectionNormalVec();
        float attackAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

        Vector2 boxCenter = (Vector2)transform.position + attackDir * 0.5f;
        Vector2 boxSize = new Vector2(1f, 1f); // 혹은 sword.range 등에서 계산

        Gizmos.color = Color.red;

        // 회전 매트릭스로 회전 적용
        Matrix4x4 rot = Matrix4x4.TRS(boxCenter, Quaternion.Euler(0, 0, attackAngle), Vector3.one);
        Gizmos.matrix = rot;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);


    }
#endif

}