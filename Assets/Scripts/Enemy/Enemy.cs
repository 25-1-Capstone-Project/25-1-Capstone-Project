using UnityEngine;


public class Enemy : MonoBehaviour
{

    [SerializeField] EnemyData enemyData;
    [SerializeField] SpriteRenderer enemySprite;
    EnemyAnimatorController enemyAnimController;
    [SerializeField] EnemyAttackPattern attackPattern;


    #region GetFunction
    public EnemyAttackPattern GetAttackPattern() => attackPattern;
    public Rigidbody2D GetRigidbody() => rb;
    public float GetSpeed() => speed;
    public EnemyAnimatorController GetAnimatorController() => enemyAnimController;
    public Transform GetPlayer() => GameManager.instance.GetPlayerTransform();

    #endregion
    public bool IsAttacking { get; set; }
    private float speed;
    int health;
    int Health
    {
        get { return health; }
        set
        {
            health = value;

            if (health < 0)
                health = 0;
        }
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
                attackPattern = EnemyManager.instance.commonEnemyAttackPatterns[0];
                break;
        }
    }
    void SetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnimController = GetComponent<EnemyAnimatorController>();
    }
    private void SetStateMachine()
    {
        // Initialize StateMachine
        StateMachine = new StateMachine<EnemyState>();
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackState(this));

        // Set initial state
        StateMachine.ChangeState<IdleState>();
    }

    void Update()
    {
        StateMachine.Update();
    }

    void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }
    void LateUpdate()
    {
        SpriteFlip();
    }
    void SpriteFlip()
    {
        enemySprite.flipX = rb.linearVelocityX == 0 ? enemySprite.flipX : rb.linearVelocity.x > 0;
    }


    public void Attack()
    {
        StartCoroutine(attackPattern.Execute(this));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // collider를 이용한 공격시
        // 데미지 함수로 대체해야함
        //설계 필요
        if (other.CompareTag("PlayerAttack"))
        {
            Debug.Log("맞음");
            Health--;
        }
    }
}