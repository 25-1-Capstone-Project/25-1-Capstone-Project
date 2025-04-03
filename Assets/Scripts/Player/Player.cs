using System.Collections;
using System.Transactions;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    Vector3 moveVec;
    Vector2 lookInput;

    [SerializeField] bool isParrying;
    [SerializeField] bool canUseParry = true;
    [SerializeField] bool canUseAttack = true;

    [SerializeField] WaitForSeconds waitAttackCoolDown;
    [SerializeField] WaitForSeconds waitParryDuration;
    [SerializeField] WaitForSeconds waitParryCoolDown;
    [SerializeField] PlayerStatus playerStat;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject ammo;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform PlayerModel;
    [SerializeField] ParticleSystem attackSlashParticle;
    [SerializeField] PlayerAnimatorController playerAnim;

    SkillPattern currentSkill;
    Vector3 direction;

    bool isDead;
    public bool GetIsDead() => isDead;
    public Vector3 Direction => direction;
    public int UIHealth => health;
    public int UIMaxHealth => playerStat.maxHealth;
    public float ParryCooldownRatio => parryCooldownTimer / playerStat.parryCooldownSec;
    private float parryCooldownTimer = 0f;

    Coroutine ParryRoutine;
    private SpriteRenderer[] spriteRenderers;

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
        playerAnim.PlayDeath();
        isDead = true;
    }
    public int ParryStack
    {
        get { return playerStat.currentParryStack; }
        set { playerStat.currentParryStack = Mathf.Max(value, 0); }
    }

    void Start()
    {
        SetComponent();
        InitPlayer();
        // 플레이어 스탯 기반으로 패리 시간·쿨타임 설정
        waitParryDuration = new WaitForSeconds(playerStat.parryDurationSec);
        waitParryCoolDown = new WaitForSeconds(playerStat.parryCooldownSec);
        waitAttackCoolDown = new WaitForSeconds(playerStat.attackCooldownSec);
        currentSkill = SkillManager.instance.SkillPatterns[0];

        // 체력 UI 테스트
        InitPlayer();
    }
    void SetComponent()
    {
        spriteRenderers = PlayerModel.GetComponentsInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<PlayerAnimatorController>();
    }
    void InitPlayer()
    {
        isDead = false;
        Health = playerStat.maxHealth;
    }

    #region 이동
    // 매 FixedUpdate마다 OnMove(PlayerInput)으로 moveVec 받아서 처리
    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + (moveVec * playerStat.speed * Time.fixedDeltaTime));
    }
    void OnMove(InputValue value)
    {
        if (isDead)
            return;

        moveVec = value.Get<Vector2>().normalized;
    }
    #endregion

    #region 방향
    // 마우스 이동으로 보는 방향 처리(마우스 위치값(OnLook)→Look()호출, 캐릭터 보는 방향 조절)
    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();

        if (lookInput == Vector2.zero || isDead)
            return;

        Look();
    }
    private void Look()
    {
        // 카메라 Z 위치 보정
        Vector3 mouseScreenPos = new Vector3(lookInput.x, lookInput.y, -Camera.main.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        // 방향
        direction = (mouseWorldPos - arrow.transform.position).normalized;
        // 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        //캐릭터의 방향
        if (direction.x < 0)
        {
            PlayerModel.rotation = Quaternion.AngleAxis(0, Vector2.up);
        }
        else
        {
            PlayerModel.rotation = Quaternion.AngleAxis(180f, Vector2.up);
        }
    }
    #endregion

    #region 공격
    void OnAttack()
    {
        if (!canUseAttack || isDead)
            return;

        StartCoroutine(AttackRoutine());
    }
    IEnumerator AttackRoutine()
    {
        canUseAttack = false;


        playerAnim.PlayAttack();

        yield return new WaitForSeconds(0.2f);
        attackSlashParticle.Play();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, playerStat.attackRange, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            Vector2 toTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(direction, toTarget);

            if (angle <= playerStat.attackAngle / 2f)
            {
                hit.GetComponent<Enemy>()?.TakeDamage(playerStat.damage);
            }
        }

        yield return waitAttackCoolDown;
        canUseAttack = true;
    }
    //attack 디버깅 용
    void OnDrawGizmos()
    {
        float range = playerStat.attackRange;
        float angle = playerStat.attackAngle;

        Vector3 forward = direction;
        Vector3 left = Quaternion.Euler(0, 0, -angle / 2) * forward;
        Vector3 right = Quaternion.Euler(0, 0, angle / 2) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + left * range);
        Gizmos.DrawLine(transform.position, transform.position + right * range);

    }
    //attack 디버깅 용


    #endregion

    #region 패링
    // 패리 키 입력 받으면 패리 가능여부 확인 후 패리 코루틴 실행
    void OnParry(InputValue value)
    {
        if (!canUseParry || isDead)
            return;

        ParryRoutine = StartCoroutine(Parry());
    }
    // 패리 코루틴, 일단 패리 사용X, 패리중O 처리→패리지속시간 기다림 뒤 ParryFailed() 호출
    IEnumerator Parry()
    {
        canUseParry = false;
        isParrying = true;
        yield return waitParryDuration;
        ParryFailed();
    }

    // 패리 성공|실패 여부에 따라 패리가능 변수 처리, 패리중→패리중X, 패리 실패 시 쿨다운 코루틴 호출
    public void ParrySuccess(Enemy enemy, Vector2 enemyDirection)
    {
        StopCoroutine(ParryRoutine);
        playerStat.currentParryStack++;
        isParrying = false;
        canUseParry = true;
        Debug.Log("패리 성공");
        enemy.StateMachine.ChangeState<KnockBackState>();
    }
    public void ParryFailed()
    {
        StopCoroutine(ParryRoutine);
        Debug.Log("패리 실패");
        isParrying = false;
        canUseParry = false;
        StartCoroutine(ParryCoolDownRoutine());

    }


    // 패리 쿨다운 코루틴, 패리 쿨만큼 기다렸다가 패리가능여부 True;
    IEnumerator ParryCoolDownRoutine()
    {
        canUseParry = false;
        parryCooldownTimer = playerStat.parryCooldownSec;
        while (parryCooldownTimer > 0)
        {
            parryCooldownTimer -= Time.deltaTime;
            yield return null;
        }
        parryCooldownTimer = 0;
        canUseParry = true;

    }
    #endregion

    // 대미지 처리 함수. 적 스크립트에서 플레이어와 적 충돌 발생 시 호출
    public void TakeDamage(int damage, Vector2 enemyDir, Enemy enemy)
    {
        if (isParrying)
        {
            float parryDot = Vector2.Dot(direction, -enemyDir);
            float threshold = Mathf.Cos(45f * Mathf.Deg2Rad); // 90도 시야
            Debug.Log(parryDot);
            if (parryDot >= threshold)
                ParrySuccess(enemy, direction);
            else
            {
                ParryFailed();
                StartCoroutine(DamagedRoutine(damage));
            }
        }
        else
            StartCoroutine(DamagedRoutine(damage));
    }
    public IEnumerator DamagedRoutine(int damage)
    {
        //playerAnim.PlayKnockBack();
        FlashOnDamage();
        Debug.Log("아야!");
        Health -= damage;
        yield return null;
    }



    #region 스킬 테스트
    void OnSkillTest(InputValue value)
    {
        if (currentSkill != null)
        {
            StartCoroutine(currentSkill.Act(this)); // 스킬 실행
        }
        else
        {
        }
        //TestSkillAct();
    }

    //void TestSkillAct()
    //{
    //    if (ParryStack <= 0)
    //    {
    //        Debug.Log("패리 스택 부족");
    //        return;
    //    }

    //    ParryStack--;

    //    GameObject fireball = Instantiate(testPrefab, transform.position, Quaternion.identity);
    //    Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

    //    if (rb != null)
    //    {
    //        Vector3 fDirection = direction;
    //        rb.linearVelocity = fDirection * 10f;
    //    }
    //}
    #endregion


    // private void Shoot()
    // {
    //     // 탄환 생성 (현재 화살표 방향 기준)
    //     GameObject ammo = Instantiate(this.ammo, arrow.transform.position, arrow.transform.rotation);

    //     // Rigidbody2D 추가 후, 발사 방향으로 속도 적용
    //     Rigidbody2D rb = ammo.GetComponent<Rigidbody2D>();

    //     if (rb != null)
    //     {
    //         rb.linearVelocity = arrow.transform.right * bulletSpeed; // 화살표가 바라보는 방향으로 발사
    //     }

    // } 

    #region FlashSprite
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    public void FlashOnDamage()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        Color[] originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
            spriteRenderers[i].color = hitColor;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = originalColors[i];
        }
    }
    #endregion

}