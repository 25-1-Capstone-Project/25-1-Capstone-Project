using System.Collections;
using UnityEngine;


public class Enemy : MonoBehaviour
{

    [SerializeField] EnemyData enemyData;
    [SerializeField] SpriteRenderer enemySprite;
    [SerializeField] EnemyAnimatorController enemyAnimController;
    [SerializeField] EnemyAttackPattern attackPattern;
    [SerializeField] ParticleSystem attackParticle;
   
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    #region GetFunction
    public EnemyAttackPattern GetAttackPattern() => attackPattern;
    public int GetDamage() => enemyData.attackDamage;
    public Rigidbody2D GetRigidbody() => rb;
    public float GetSpeed() => speed;
    public EnemyAnimatorController GetAnimatorController() => enemyAnimController;
    public Transform GetPlayer() => PlayerScript.Instance.GetPlayerTransform();

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
            case EEnemyType.Sword:
                attackPattern = EnemyManager.Instance.commonEnemyAttackPatterns[0];
                break;
            case EEnemyType.Spear:
                attackPattern = EnemyManager.Instance.commonEnemyAttackPatterns[1];
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
    public bool CheckAttackRange()
    {
         // 플레이어가 가까우면 공격 상태로 전환
        if (GetDirectionVec().magnitude < enemyData.attackRange) 
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
        StateMachine.ChangeState<DamagedState>();
        Health -= damage;
        Debug.Log("적 아야");
    }
    public void KnockBack(float knockBackForce)
    {
        enemyAnimController.PlayKnockBack();
        rb.linearVelocity = -GetDirectionNormalVec() * knockBackForce;
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
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        switch (enemyData.eEnemyType)
        {
            case EEnemyType.Sword:
                Gizmos.color = Color.blue;
                Vector2 attackDir = GetDirectionNormalVec();
                float attackAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

                Vector2 boxCenter = (Vector2)transform.position + attackDir * 0.5f;
                Vector2 boxSize = new Vector2(1f, 1f); // 혹은 sword.range 등에서 계산

                Gizmos.color = Color.red;

                // 회전 매트릭스로 회전 적용
                Matrix4x4 rot = Matrix4x4.TRS(boxCenter, Quaternion.Euler(0, 0, attackAngle), Vector3.one);
                Gizmos.matrix = rot;
                Gizmos.DrawWireCube(Vector2.zero, new Vector2(boxSize.x, boxSize.y));

                break;
            case EEnemyType.Spear:
            
            
                break;
        }


    }
#endif

}