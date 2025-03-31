using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    private float speed;
    int health;
    private float attackSpeed;
    private float attackDamage;
    private Rigidbody2D rb;

    // StateMachine Property
    public StateMachine<EnemyState> StateMachine { get; private set; }

   void Start()
    {

        SetComponents();
        DataInit();
        SetStateMachine();
    }
    void DataInit()
    {
        attackSpeed = enemyData.attackDamage;
        attackSpeed = enemyData.attackSpeed;
        speed = enemyData.moveSpeed;
        health = enemyData.maxHealth;
    }
    void SetComponents(){
        rb = GetComponent<Rigidbody2D>();
    }
 

    private void SetStateMachine()
    {
        // Initialize StateMachine
        StateMachine = new StateMachine<EnemyState>();
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new MoveState(this));
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

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    public float GetSpeed()
    {
        return speed;
    }
}