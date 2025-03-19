using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
  
    private Rigidbody2D rb;

    // StateMachine Property
    public StateMachine<EnemyState> StateMachine { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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

    // Public methods to expose data or perform actions
    public Transform GetPlayerTransform()
    {
        return GameManager.instance.playerTransform;
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