using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 이동, 공격, 패링, 대시 등을 처리하는 스크립트입니다.
/// 플레이어의 상태를 관리하고, 애니메이션과 상호작용합니다.
/// 씬 전환 시 플레이어의 상태를 저장하고 불러오는 기능도 포함되어 있습니다.
/// 이 스크립트는 Singleton 패턴을 사용합니다.
/// </summary>
public class PlayerScript : Singleton<PlayerScript>
{
    protected override void Awake()
    {
        base.Awake();
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
    bool isGod = false; // 무적 상태

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
            UIManager.Instance.playerStatUI.UI_HPBarUpdate();
        }
    }
    public int UIHealth => health;
    public int UIMaxHealth => stats.maxHealth;

    [Header("=====패링 옵션=====")]
    [SerializeField] bool canUseParry = true;
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
                UIManager.Instance.parryStackUI.RemoveAllParryStackIcon();
            }
            // 증가 → 아이콘 추가
            else if (value > previous)
            {
                UIManager.Instance.parryStackUI.AddParryStackIcon();
            }
            // 감소 → 아이콘 제거
            else if (value < previous)
            {
                int delta = previous - value;
                UIManager.Instance.parryStackUI.RemoveParryStackIcon(delta);
            }


        }
    }
    public void SetParryStack(int max) { stats.maxParryStack = max; UIManager.Instance.parryStackUI.SetMaxParryStack(); }


    [Header("=====대시 옵션=====")]
    [SerializeField] float dashDistance = 5f;
    [SerializeField] bool canUseDash = true;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;


    [Header("=====플래시 옵션=====")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;



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

    public void InitPlayer()
    {
        stats.ApplyBase(playerData); // 원본 데이터를 복사

        // 스킬 불러오기?

        SkillSetting(0);

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


        // 체력 UI 테스트

    }

    void LateUpdate()
    {
        if (isDead || isAttacking || isParrying || isDashing) return;

        playerAnim.UpdateMovement(moveVec);

    }

    void SetComponent()
    {
        spriteRenderers = PlayerModel.GetComponentsInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<PlayerAnimatorController>();
    }

    #region 이동
    void OnMove(InputValue value)
    {
        moveVec = value.Get<Vector2>().normalized;
    }
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
        playerAnim.PlayDash(moveVec);
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
        EffectPooler.Instance.SpawnFromPool("AttackSlashParticle", arrow.transform.position, arrow.transform.rotation);
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

        yield return new WaitForSeconds(stats.attackCooldownSec);
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
        // 패리 지속시간이 끝나면 패리중X 처리
        yield return new WaitForSeconds(stats.parryDurationSec);
        isParrying = false;

        // 패리 쿨타임이 끝나면 패리 가능여부 True 처리
        yield return new WaitForSeconds(stats.parryCooldownSec);
        canUseParry = true;
    }

    // 패리 성공|실패 여부에 따라 패리가능 변수 처리, 패리중→패리중X, 패리 실패 시 쿨다운 코루틴 호출
    public void ParrySuccess(Enemy enemy)
    {
        StopCoroutine(ParryRoutine);

        if (ParryStack < stats.maxParryStack)
        {
            ParryStack++;
        }
        // 주먹구구식으로 땜질해 뒀는데 고찰이 필요...
        if (ParryStack == stats.maxParryStack)
        {
            currentSkill.ResetCooldown();
            if (cooldownRoutine != null)
            {
                StopCoroutine(cooldownRoutine);
                cooldownRoutine = null;
                UIManager.Instance.skillUI.UpdateCooldown(0f);
            }
        }

        isParrying = false;
        canUseParry = true;
        enemy?.StateMachine.ChangeState<ParryState>();
        StartCoroutine(ParryEffect());

    }
    public IEnumerator ParryEffect()
    {

        EffectPooler.Instance.SpawnFromPool("ParryEffect", transform.position + (direction / 2), Quaternion.identity);
        AudioManager.Instance.PlaySFX("ParrySuccess");
        isGod = true;
        GameManager.Instance.SetTimeScale(0.5f);
        yield return FadeController.Instance.FadeOut(Color.white, 0.05f, 0.01f);
        yield return FadeController.Instance.FadeIn(Color.white, 0.05f, 0.01f);
        GameManager.Instance.SetTimeScale(1);

        yield return new WaitForSeconds(0.5f);
        isGod = false;

    }
    //잠시 삭제 (쿨타임 하나로 묶어버림)
    // public void ParryFailed()
    // {
    //     StopCoroutine(ParryRoutine);
    //     Debug.Log("패리 실패");
    //     isParrying = false;
    //     canUseParry = false;

    // }


    // 패리 쿨다운 코루틴, 패리 쿨만큼 기다렸다가 패리가능여부 True;
    #endregion

    #region 데미지 처리

    // 대미지 처리 함수. 적 스크립트에서 플레이어와 적 충돌 발생 시 호출
    public void TakeDamage(int damage, Vector2 enemyDir, Enemy enemy = null)
    {
        if (isDead) return;
        if (isGod) return;

        if (isParrying)
        {
            float parryDot = Vector2.Dot(direction, -enemyDir);
            float threshold = Mathf.Cos(45f * Mathf.Deg2Rad); // 90도 시야
            Debug.Log(parryDot);
            if (parryDot >= threshold)
                ParrySuccess(enemy);
            else
            {
                // ParryFailed();
                StartCoroutine(DamagedRoutine(damage));
            }
        }
        else
            StartCoroutine(DamagedRoutine(damage));
    }
    public IEnumerator DamagedRoutine(int damage)
    {
        isGod = true;
        playerAnim.PlayKnockBack();
        FlashOnDamage();
        Debug.Log("아야!");
        Health -= damage;
        yield return new WaitForSeconds(1f);
        isGod = false;
    }
    #endregion

    #region 스킬
    Coroutine cooldownRoutine;

    // 스킬 셋팅
    void SkillSetting(int skillNum)
    {
        currentSkill = SkillManager.Instance.SkillPatterns[skillNum];

        if (currentSkill == null)
        {
            SetParryStack(0);
        }
        else
        {
            // 땜질2
            SetParryStack(currentSkill.ultimateCost);
            UIManager.Instance.parryStackUI.SyncParryIcons(ParryStack);
            UIManager.Instance.skillUI.UpdateSkillIcon(currentSkill.skillIcon);
        }
    }



    // 스킬 키 입력
    void OnSkill(InputValue value)
    {
        var skillType = currentSkill.ParryStackCheck();

        if (currentSkill == null || skillType == SkillType.Empty)
            return;

        if (isDead || isDashing || isParrying)
            return;

        // 스킬 쿨타임 체크
        if (!currentSkill.IsCooldownReady())
            return;

        switch (skillType)
        {
            case SkillType.Common:
                ParryStack -= currentSkill.commonCost;
                currentSkill.SetCooldown();
                StartCoroutine(currentSkill.CommonSkill(this));
                if (cooldownRoutine != null)
                    StopCoroutine(cooldownRoutine);
                cooldownRoutine = StartCoroutine(CooldownRoutine());
                break;
            case SkillType.Ultimate:
                ParryStack -= currentSkill.ultimateCost;
                currentSkill.ResetCooldown();
                StartCoroutine(currentSkill.UltimateSkill(this));
                break;
                //case SkillType.Empty:
                //    Debug.Log("스킬 사용 불가");
                //    break;

        }
    }

    IEnumerator CooldownRoutine()
    {
        float duration = currentSkill.cooldown;
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float elapsed = Time.time - startTime;
            float ratio = Mathf.Clamp01(1f - (elapsed / duration));
            UIManager.Instance.skillUI.UpdateCooldown(ratio);
            yield return null;
        }

        UIManager.Instance.skillUI.UpdateCooldown(0f);
        cooldownRoutine = null;
    }
    #endregion

    public void Dead()
    {
        playerAnim.PlayDeath();
        isDead = true;
        rb.linearVelocity = Vector2.zero;
    }


    #region 인벤토리

    void OnInventory(InputValue value)
    {
        //SkillSetting(1);
        Time.timeScale = 0f;
        UIManager.Instance.skillSelect.ShowSkillWindow(OnSkillSelected);
    }

    void OnSkillSelected(int index)
    {
        SkillSetting(index);
    }

    #endregion


    #region FlashSprite


    public void FlashOnDamage()
    {
        StartCoroutine(FlashRoutine(hitColor));
    }

    private IEnumerator FlashRoutine(Color color)
    {

        for (int i = 0; i < spriteRenderers.Length; i++)
        {

            spriteRenderers[i].color = color;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = Color.white;
        }
    }
    public void FlashParry()
    {
        StartCoroutine(FlashRoutine(Color.blue));
    }


    #endregion

}