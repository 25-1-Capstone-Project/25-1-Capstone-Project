using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] SpriteRenderer enemySprite;
    EnemyAnimatorController enemyAnimController;
    private float speed;
    int health;
    float attackSpeed;
    float attackDamage;
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
        attackDamage = enemyData.attackDamage;
        attackSpeed = enemyData.attackSpeed;
        speed = enemyData.moveSpeed;
        health = enemyData.maxHealth;
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
    void SpriteFlip(){
         enemySprite.flipX = rb.linearVelocityX == 0 ? enemySprite.flipX :rb.linearVelocity.x > 0 ;
    }
    #region GetFunction
    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public EnemyAnimatorController GetAnimatorController()
    {
        return enemyAnimController;
    }
    #endregion
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            health--;
        }
    }
}