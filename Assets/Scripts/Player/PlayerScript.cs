using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    [Header("방향 관련")]
    Vector2 moveVec;
    Vector2 lookInput;
    Vector3 direction;
    public Vector3 Direction => direction;


    [Header("=====플레이어 상태=====")]
    bool canUseAttack = true;
    bool isParrying = false;
    bool isDead = false;
    bool isAttacking = false;
    bool isDashing = false;

    [Header("=====체력=====")]
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
    public int UIHealth => health;
    public int UIMaxHealth => stats.maxHealth;

    [Header("=====패링 옵션=====")]
    [SerializeField] bool canUseParry = true;
    [SerializeField] GameObject parryEffectPrefab; // 패링 이펙트 프리팹
    public float ParryCooldownRatio => parryCooldownTimer / stats.attackCooldownSec;
    private float parryCooldownTimer = 0f;
    Coroutine ParryRoutine;
    public int ParryStack
    {
        get => stats.currentParryStack;
        set
        {
            int previous = stats.currentParryStack;
            stats.currentParryStack = value;

            // 0이 된 경우 전체 제거
            if (value == 0 && previous > 0)
            {
                UIManager.instance.parryStackUI.RemoveAllParryStackIcon();
            }
            // 증가 → 아이콘 추가
            else if (value > previous)
            {
                UIManager.instance.parryStackUI.AddParryStackIcon();
            }
            // 감소 → 아이콘 제거
            else if (value < previous)
            {
                int delta = previous - value;
                UIManager.instance.parryStackUI.RemoveParryStackIcon(delta);
            }


        }
    }
    public void SetParryStack(int max) { stats.maxParryStack = max; UIManager.instance.parryStackUI.SetMaxParryStack(); }


    [Header("=====대시 옵션=====")]
    [SerializeField] float dashDistance = 5f;
    [SerializeField] bool canUseDash = true;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;


    [Header("=====플래시 옵션=====")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("=====대기 시간=====")]
    WaitForSeconds waitAttackCoolDown;
    WaitForSeconds waitParryDuration;


    [Header("=====컴포넌트=====")]
    [SerializeField] PlayerData playerData;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject ammo;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform PlayerModel;
    [SerializeField] ParticleSystem attackSlashParticle;
    [SerializeField] PlayerAnimatorController playerAnim;
    SkillPattern currentSkill;
    private SpriteRenderer[] spriteRenderers;
    public PlayerRuntimeStats stats = new PlayerRuntimeStats();

    void InitPlayer()
    {
        stats.ApplyBase(playerData); // 원본 데이터를 복사

        currentSkill = SkillManager.instance.SkillPatterns[0]; // 현재 스킬 가져오기
        if (currentSkill == null)
            SetParryStack(0);
        else
            SetParryStack(currentSkill.ultimateCost);

        isDead = false;
        Health = stats.maxHealth;
    }

    #region GetFunction
    public PlayerRuntimeStats GetPlayerRuntimeStats() => stats;
    public bool GetIsDead() => isDead;
    public Transform GetPlayerTransform()
    {
        return transform;
    }
 

    #endregion

    void Start()
    {
        InitPlayer();
        SetComponent();

        // 플레이어 스탯 기반으로 패리 시간·쿨타임 설정
        waitParryDuration = new WaitForSeconds(stats.parryDurationSec);
        waitAttackCoolDown = new WaitForSeconds(stats.attackCooldownSec);

        // 체력 UI 테스트

    }
    void LateUpdate()
    {
        if (isDead || isAttacking || isParrying || isDashing) return;

        playerAnim.UpdateMovement(moveVec);
        FlipX();
    }

    void SetComponent()
    {
        spriteRenderers = PlayerModel.GetComponentsInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<PlayerAnimatorController>();
    }

    #region 이동
    // 매 FixedUpdate마다 OnMove(PlayerInput)으로 moveVec 받아서 처리
    private void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        if (isDashing || isDead || isAttacking || isParrying)
            return;

        rb.linearVelocity = moveVec * stats.speed;

        //기존방식
        //     rb.MovePosition(transform.position + (moveVec * playerStat.speed * Time.fixedDeltaTime));

    }
    void OnMove(InputValue value)
    {
        if (isDead || isDashing)
            return;

        moveVec = value.Get<Vector2>().normalized;
    
    }

    void OnDash()
    {
        if (isDead || !canUseDash || moveVec == Vector2.zero)
            return;
        StartCoroutine(DashCoroutine());
    }

    IEnumerator DashCoroutine()
    {
        isDashing = true;
        canUseDash = false;
        playerAnim.PlayDash();
        // 대시 방향과 힘 설정
        rb.linearVelocity = moveVec.normalized * (dashDistance / dashDuration);

        // playerAnim.PlayDash();

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canUseDash = true;
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
    }
    private void FlipX()
    {
        if (moveVec.x == 0)
            return;

        //캐릭터의 방향
        if (moveVec.x > 0)
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
        if (!canUseAttack || isDead || isDashing || isParrying)
            return;

        StartCoroutine(AttackRoutine());
    }
    IEnumerator AttackRoutine()
    {

        isAttacking = true;
        canUseAttack = false;
        rb.linearVelocity = Vector2.zero;
        playerAnim.PlayAttack();

        yield return new WaitForSeconds(0.2f);
        attackSlashParticle.Play();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stats.attackRange, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            Vector2 toTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(direction, toTarget);

            if (angle <= stats.attackAngle / 2f)
            {
                hit.GetComponent<Enemy>()?.TakeDamage(stats.damage);
            }
        }

        yield return waitAttackCoolDown;
        isAttacking = false;
        canUseAttack = true;
    }
    //attack 디버깅 용
    void OnDrawGizmos()
    {
        float range = stats.attackRange;
        float angle = stats.attackAngle;

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
        if (!canUseParry || isDead || isAttacking || isDashing)
            return;

        playerAnim.PlayParry();
        FlashParry();
        ParryRoutine = StartCoroutine(Parry());
    }
    // 패리 코루틴, 일단 패리 사용X, 패리중O 처리→패리지속시간 기다림 뒤 ParryFailed() 호출
    IEnumerator Parry()
    {
        canUseParry = false;
        isParrying = true;
        rb.linearVelocity = Vector2.zero;
        yield return waitParryDuration;
        ParryFailed();
    }

    // 패리 성공|실패 여부에 따라 패리가능 변수 처리, 패리중→패리중X, 패리 실패 시 쿨다운 코루틴 호출
    public void ParrySuccess(Enemy enemy)
    {
        StopCoroutine(ParryRoutine);

        if (ParryStack < stats.maxParryStack)
        {
            ParryStack++;
        }

        isParrying = false;
        canUseParry = true;
        enemy.StateMachine.ChangeState<ParryState>();
        ParryEffect();

    }
    public void ParryEffect()
    {
        GameObject effect = Instantiate(parryEffectPrefab, transform.position + (direction / 2), Quaternion.identity);
        Destroy(effect, 0.5f);
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
        parryCooldownTimer = stats.parryCooldownSec;
        while (parryCooldownTimer > 0)
        {
            parryCooldownTimer -= Time.deltaTime;
            yield return null;
        }
        parryCooldownTimer = 0;
        canUseParry = true;

    }
    #endregion

    #region 데미지 처리

    // 대미지 처리 함수. 적 스크립트에서 플레이어와 적 충돌 발생 시 호출
    public void TakeDamage(int damage, Vector2 enemyDir, Enemy enemy)
    {
        if (isParrying)
        {
            float parryDot = Vector2.Dot(direction, -enemyDir);
            float threshold = Mathf.Cos(45f * Mathf.Deg2Rad); // 90도 시야
            Debug.Log(parryDot);
            if (parryDot >= threshold)
                ParrySuccess(enemy);
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
        playerAnim.PlayKnockBack();
        FlashOnDamage();
        Debug.Log("아야!");
        Health -= damage;
        yield return null;
    }
    #endregion

    #region 스킬 테스트
    void OnSkillTest(InputValue value)
    {
        var skillType = currentSkill.ParryStackCheck();

        if (currentSkill == null || skillType == SkillType.Empty)
            return;

        switch (skillType)
        {
            case SkillType.Common:
                ParryStack -= currentSkill.commonCost;

                StartCoroutine(currentSkill.CommonSkill(this));
                break;
            case SkillType.Ultimate:
                ParryStack -= currentSkill.ultimateCost;
                StartCoroutine(currentSkill.UltimateSkill(this));
                break;
            case SkillType.Empty:
                Debug.Log("스킬 사용 불가");
                break;

        }


        //TestSkillAct();
    }
    public void Dead()
    {
        playerAnim.PlayDeath();
        isDead = true;
        rb.linearVelocity = Vector2.zero;
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


    public void FlashOnDamage()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {

        for (int i = 0; i < spriteRenderers.Length; i++)
        {

            spriteRenderers[i].color = hitColor;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = Color.white;
        }
    }
    public void FlashParry()
    {
        StartCoroutine(FlashParryRoutine());
    }

    private IEnumerator FlashParryRoutine()
    {

        for (int i = 0; i < spriteRenderers.Length; i++)
        {

            spriteRenderers[i].color = Color.blue;
        }

        yield return waitParryDuration;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = Color.white;
        }
    }
    #endregion

}