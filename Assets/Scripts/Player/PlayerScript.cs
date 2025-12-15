using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Splines.ExtrusionShapes;



/// <summary>
/// 플레이어의 이동, 공격, 패링, 대시 등을 처리하는 스크립트입니다.
/// 플레이어의 상태를 관리하고, 애니메이션과 상호작용합니다.
/// 씬 전환 시 플레이어의 상태를 저장하고 불러오는 기능도 포함되어 있습니다.
/// 이 스크립트는 Singleton 패턴을 사용합니다.
/// </summary>
public class PlayerScript : MonoBehaviour
{

    [Header("방향 관련")]
    Vector2 moveVec;
    Vector2 lookInput;
    Vector3 direction;
    Vector2 takeAttackDirection;
    public Vector3 Direction => direction;
    public Vector2 Direction2D => direction;
    [Header("=====플레이어 상태=====")]
    //[SerializeField] bool canUseAttack = false;

    bool isParrying = false;
    bool isDead = false;
    bool isAttacking = false;
    bool isDashing = false;
    bool isGod = false; // 무적 상태

    bool canMove = true;
    private PlayerInput playerInput;


    [Header("=====체력=====")]

    public int Health
    {
        get { return stats.currentHealth; }
        set
        {
            int health = value;

            if (health > stats.maxHealth)
            {
                health = stats.maxHealth;
            }
            if (health <= 0)
            {
                health = 0;
                Dead();
            }
            else if (health < stats.currentHealth)
            {
                OnDamaged();
            }

            stats.currentHealth = health;

            UIManager.Instance.playerStatUI.UI_HPBarUpdate(stats.currentHealth, stats.maxHealth);
        }
    }
    public void SetMaxHealth()
    {
        Health = stats.maxHealth;
    }

    [Header("=====패링 옵션=====")]
    [SerializeField] EnemyBase targetEnemy;
    [SerializeField] bool canUseParry = true;
    public void SetCanUseParry(bool value) => canUseParry = value;
    public float ParryCooldownRatio => parryCooldownTimer / stats.attackCooldownSec;
    private float parryCooldownTimer = 0f;
    Coroutine ParryRoutine;

    // 증강
    public event Action OnParrySuccess;
    public event Action OnParryInput;
    public event Action OnAttackInput;

    [Header("=====대시 옵션=====")]
    [SerializeField] float dashDistance = 6f;
    [SerializeField] bool canUseDash = true;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;


    [Header("=====컴포넌트=====")]
    [SerializeField] PlayerData playerData;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject ammo;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform PlayerModel;
    [SerializeField] PlayerAnimatorController playerAnim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem skillParticle;

    private PlayerRuntimeStats stats = new PlayerRuntimeStats();
    public PlayerRuntimeStats Stats => stats;

    public void InitPlayer()
    {

        CameraManager.Instance.SetLensSize(6.5f);
        SetComponent();
        stats.ApplyBase(playerData); // 원본 데이터를 복사
        isDead = false;
        playerAnim.SetDeath(isDead);
        playerInput.enabled = true;
        Health = stats.maxHealth;
        canMove = true;
        UIManager.Instance.playerStatUI.HPUIInit(stats.maxHealth);
        SetActivePlayerInput(true);
    }
    public void DestroyPlayer()
    {
        Destroy(gameObject);
    }
    #region GetSetFunction
    public PlayerRuntimeStats GetPlayerRuntimeStats() => stats;
    public bool GetIsDead() => isDead;
    public Transform GetPlayerTransform()
    {
        return transform ? transform : null;
    }
    public void SetPlayerPosition(Vector2 target)
    {
        transform.position = target;
    }
    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }
    public void SetActivePlayerInput(bool isActive)
    {
        playerInput.enabled = isActive;
    }
    public void OnlyParryAfterTime(float time) => StartCoroutine(OnlyParryRoutine(time));

    IEnumerator OnlyParryRoutine(float time)
    {
        if (ParryRoutine != null)
            StopCoroutine(ParryRoutine);
        canUseParry = false;

        yield return new WaitForSecondsRealtime(time);
        UIManager.Instance.guideUI.SetActiveGuideUI(true, "공격 방향으로 [우클릭] 패링!");
        canUseParry = true;

    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        rb.linearVelocity = Vector2.zero;
    }
    #endregion

    void Update()
    {
        if (!isPressed) return;
        if (isDead || isDashing || isParrying || isAttacking) return;

        // 홀드 시간 지나면 조준 모드 진입
        if (!isAiming && (Time.time - pressStartTime) >= enterAimHoldTime)
        {
            isAiming = true;
            StartCoroutine(UpdateThrowAim());
        }

    }
    IEnumerator UpdateThrowAim()
    {
        if (!targetEnemy) yield break;
        if (!targetEnemy.canThrow) yield break;
        CameraManager.Instance.SetLensSize(6f);
        yield return null;
        Vector2 origin = targetEnemy.transform.position;
        transform.position = origin;
        GameManager.Instance.SetTimeScale(0f);
        targetEnemy.GetComponent<Collider2D>().isTrigger = true;
        while (isAiming)
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Vector2 dir = mouseWorld - origin;
            aimDir = dir.normalized;
            targetEnemy.transform.position = origin + aimDir * 1f;
            yield return null;
        }
    }
    void LateUpdate()
    {
        if (!canMove || isDead || isAttacking || isParrying || isDashing) return;

        playerAnim.UpdateMovement(moveVec);

    }

    void SetComponent()
    {

        spriteRenderer = PlayerModel.GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<PlayerAnimatorController>();
        playerInput = GetComponent<PlayerInput>();
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
        if (!canMove || isDashing || isDead || isParrying)
            return;

        rb.linearVelocity = moveVec * stats.speed;
    }
    void OnDash()
    {
        if (isDead || !canUseDash || moveVec == Vector2.zero)
            return;
        StartCoroutine(DashCoroutine());
        AudioManager.Instance.PlaySFX("Dash");
    }


    IEnumerator DashCoroutine()
    {

        gameObject.layer = LayerMask.NameToLayer("PlayerDash"); // 대시 중 플레이어 레이어 변경
        isDashing = true;
        canUseDash = false;

        playerAnim.PlayDash(moveVec);
        rb.linearVelocity = moveVec.normalized * (dashDistance / dashDuration);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        isDashing = false;

        gameObject.layer = LayerMask.NameToLayer("Player");
        yield return new WaitForSeconds(dashCooldown);


        canUseDash = true;
    }



    #endregion

    #region 방향
    // 마우스 이동으로 보는 방향 
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
        direction = (mouseWorldPos - transform.position).normalized;
        // 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    #endregion
    #region 공격
    [SerializeField] LayerMask enemyMask;
    [SerializeField] float enterAimHoldTime = 0.15f;

    bool isPressed;
    bool isAiming;
    float pressStartTime;
    Vector2 aimDir;

    // 외부에서 읽고 싶으면 이렇게만 노출
    public bool IsAiming => isAiming;
    public Vector2 AimDir => aimDir;

    public void OnAttack(InputValue value)
    {
        if (isDead || isDashing || isParrying || isAttacking) return;

        if (value.isPressed)
        {
            // Press 시작
            isPressed = true;
            isAiming = false;
            pressStartTime = Time.time;
            SearchTarget();
            return;
        }

        if (targetEnemy == null)
        {
            isPressed = false;
            isAiming = false;
            return;
        }
        // Release
        isPressed = false;

        if (isAiming)
        {
            //놓으면 발사
            GameManager.Instance.SetTimeScale(1f);
            CameraManager.Instance.SetLensSize(6.5f);
            Throw(aimDir);

        }
        else
        {
            Attack();
        }

        isAiming = false;
    }
    void Throw(Vector2 dir)
    {
        isAiming = false;
        isPressed = false;
        targetEnemy.Throw(dir);
        CameraManager.Instance.CameraShake(10f, 0.3f);
        playerAnim.PlayAttack();
    }

    void Attack()
    {
        if (isDead || isDashing || isParrying || isAttacking)
            return;

        // 스턴 상태의 적이 있으면 공격 실행
        if (targetEnemy != null)
        {
            OnAttackInput?.Invoke();

            AudioManager.Instance.PlaySFX("Damaged");
            StartCoroutine(AttackRoutine());
        }

    }
    void SearchTarget()
    {
        targetEnemy = null;
        // 클릭 위치를 월드좌표로 변환
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 클릭 위치에서 지름 1(반지름 0.5)의 원 범위 검사
        Collider2D[] hits = Physics2D.OverlapCircleAll(mouseWorld, 0.5f, enemyMask);

        if (hits.Length == 0)
            return;

        // 스턴 상태인 적 중 가장 가까운 적 찾기

        float closestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null && enemy.CheckStunned())
            {
                float distance = Vector2.Distance(mouseWorld, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetEnemy = enemy;
                }
            }
        }
    }
    IEnumerator AttackRoutine()
    {
        GameManager.Instance.SetTimeScale(1f);
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        playerAnim.PlayAttack();

        CameraManager.Instance.CameraShake(10f, 0.3f);

        CameraManager.Instance.SetLensSize(5f);
        Vector2 toEnemyDirection = -targetEnemy.GetDirectionNormalVec();
        float angle = Mathf.Atan2(toEnemyDirection.y, toEnemyDirection.x) * Mathf.Rad2Deg;

        RaycastHit2D hit = Physics2D.Raycast(targetEnemy.transform.position, toEnemyDirection, 1f, LayerMask.GetMask("Wall"));
        if (hit.collider != null)
        {
            transform.position = hit.point - toEnemyDirection * 0.5f;
        }
        else
        {
            transform.position = (Vector2)targetEnemy.transform.position + toEnemyDirection;
        }
        attackEffect = EffectPooler.Instance.SpawnFromPool("AttackEffect", transform.position, Quaternion.Euler(0, 0, angle));
        AudioManager.Instance.PlaySFX("AttackHit");

        targetEnemy.TakeDamage(1);
        GameManager.Instance.SetTimeScale(0);
        yield return new WaitForSecondsRealtime(0.2f);
        GameManager.Instance.SetTimeScale(1f);
        CameraManager.Instance.SetLensSize(6.5f);
        attackEffect.SetActive(false);
        isGod = false;
        canMove = true;
        isAttacking = false;
        targetEnemy = null;

    }


    #endregion

    #region 패링
    // 패리 키 입력 받으면 패리 가능여부 확인 후 패리 코루틴 실행
    void OnParry(InputValue value)
    {
        if (!canUseParry || isDead || isAttacking || isDashing)
            return;

        isGod = false;
        OnParryInput?.Invoke();
        playerAnim.PlayAttack();
        AudioManager.Instance.PlaySFX("ParryTry");
        ParryRoutine = StartCoroutine(Parry());
    }
    // 패리 코루틴, 일단 패리 사용X, 패리중O 처리→패리지속시간 기다림 뒤 ParryFailed() 호출
    IEnumerator Parry()
    {
        canUseParry = false;
        isParrying = true;
        rb.linearVelocity = Vector2.zero;

        canMove = false;
        // CheckInteractObject();

        // 패리 지속시간이 끝나면 패리중X 처리
        yield return new WaitForSeconds(stats.parryDurationSec);
        isParrying = false;
        canMove = true;
        playerAnim.PlayIdle();
        // 패리 쿨타임이 끝나면 패리 가능여부 True 처리
        yield return new WaitForSeconds(stats.parryCooldownSec);
        canUseParry = true;

    }

    // void CheckInteractObject()
    // {
    //     float checkRange = stats.attackRange;
    //     float checkAngle = stats.attackAngle;

    //     Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRange, LayerMask.GetMask("Interactable"));

    //     foreach (var hit in hits)
    //     {
    //         if (hit != null)
    //         {
    //             Vector2 toTarget = (hit.transform.position - transform.position).normalized;
    //             float angle = Vector2.Angle(Direction, toTarget);

    //             if (angle <= checkAngle / 2f)
    //             {
    //                 hit.GetComponent<Interactable>().Interact();
    //             }
    //         }
    //     }
    // }


    // 근접 패링
    public void ParrySuccess(EnemyBase enemy)
    {
        StopCoroutine(ParryRoutine);
        //PerformParryPulse(enemy);

        OnParrySuccess?.Invoke();


        canUseParry = true;

        enemy.Stamina--;

        StartCoroutine(ParryEffect());

        isParrying = false;
    }
    //원거리 패링
    public void ParrySuccess(EnemyAttackBase enemyAttack)
    {
        StopCoroutine(ParryRoutine);
        // PerformParryPulse(null);


        OnParrySuccess?.Invoke();
        enemyAttack.gameObject.SetActive(true);
        enemyAttack.gameObject.tag = "PlayerAttack";
        enemyAttack.SetDirectionVec(direction); // 방향 반전

        isParrying = false;
        canUseParry = true;
        StartCoroutine(ParryEffect(true));

    }
    public IEnumerator ParryEffect(bool projectile = false)
    {
        CameraManager.Instance.CameraShake(5f, 0.2f);
        EffectPooler.Instance.SpawnFromPool("ParryEffect", transform.position + (direction / 2), Quaternion.identity);
        AudioManager.Instance.PlaySFX("Parry" + UnityEngine.Random.Range(0, 2));

        if (!projectile)
            yield return StartCoroutine(ParryEffectRoutine());

        canMove = true;

        isGod = false;
    }
    public IEnumerator ParryEffectRoutine()
    {
        CameraManager.Instance.SetLensSize(6f);
        ShaderManager.Instance.CallShockWave();
        yield return new WaitForSecondsRealtime(0.05f);
        GameManager.Instance.SetTimeScale(0);

        //   yield return FadeController.Instance.FadeIn(Color.white, 0.1f, 0.3f);
        yield return new WaitForSecondsRealtime(0.25f);
        //ShaderManager.Instance.CallShockWave();
        GameManager.Instance.SetTimeScale(1);
        CameraManager.Instance.SetLensSize(6.5f);
    }
    GameObject attackEffect;


    //Room 클리어 시 연출 변경 후 정상 작동을 위해 임시로 만든 함수입니다. 빠른 개발 용
    public void ClearSet()
    {
        attackEffect?.SetActive(false);
        isGod = false;
        isAttacking = false;
    }


    #endregion

    #region 데미지 처리

    // 근거리 대미지 처리 함수. 
    public void TakeAttack(EnemyBase enemy)
    {
        if (isDead) return;
        if (isGod) return;

        takeAttackDirection = enemy.GetDirectionNormalVec();
        if (isParrying)
        {
            float parryDot = Vector2.Dot(direction, -takeAttackDirection);
            float threshold = Mathf.Cos(30f * Mathf.Deg2Rad);

            if (parryDot >= threshold)
                ParrySuccess(enemy);
            else
            {

                Health -= 1;
            }
        }
        else
            Health -= 1;

    }

    // 원거리 대미지 처리 함수.
    public void TakeAttack(EnemyAttackBase enemyAttack)
    {
        if (isDead) return;
        if (isGod) return;

        takeAttackDirection = enemyAttack.GetDirectionNormalVec();

        if (isParrying && enemyAttack.CanParry)
        {

            float parryDot = Vector2.Dot(direction, -takeAttackDirection);
            float threshold = Mathf.Cos(45f * Mathf.Deg2Rad); // 90도 시야

            if (parryDot >= threshold)
                ParrySuccess(enemyAttack);
            else
            {
                if (enemyAttack is ProjectileEnemyAttack)
                {
                    enemyAttack.gameObject.SetActive(false);
                }
                Health -= 1;
            }
        }
        else
        {
            if (enemyAttack is ProjectileEnemyAttack)
            {
                enemyAttack.gameObject.SetActive(false);
            }
            Health -= 1;

        }
    }

    public void KnockBack(Vector2 forceDir, float knockBackForce)
    {

        rb.AddForce(forceDir * knockBackForce, ForceMode2D.Impulse);

    }

    public void OnDamaged()
    {
        StartCoroutine(DamagedRoutine());
    }
    public IEnumerator DamagedRoutine()
    {
        playerInput.enabled = false;
        isGod = true;

        AudioManager.Instance.PlaySFX("Hit");
        playerAnim.PlayDamaged();


        rb.linearVelocity = Vector2.zero;

        playerInput.enabled = true;
        yield return StartCoroutine(FlashInvincible());
        isGod = false;
    }



    #endregion
    public void Dead()
    {
        if (isDead) return;
        isDead = true;

        StartCoroutine(DeadRoutine());
    }

    IEnumerator DeadRoutine()
    {
        playerAnim.SetDeath(true);
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySFX("Hit");
        CameraManager.Instance.CameraShake(10, 0.5f);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSecondsRealtime(0.5f);
        GameManager.Instance.SetTimeScale(0);
        UIManager.Instance.deadUI.SetActiveDeadInfoPanel(true);
        UIManager.Instance.deadUI.PlayMaskShrink();
        yield return new WaitForSecondsRealtime(1.3f);
        AudioManager.Instance.PlaySFX("GameOverMelody");
        AudioManager.Instance.PlaySFX("GameOver");

    }


    #region FlashSprite

    public IEnumerator FlashInvincible()
    {
        StartCoroutine(FlashVignette());
        float elapsed = 0f;
        bool fadingOut = true;
        Color baseColor = spriteRenderer.color;


        while (elapsed < playerData.invincibleDuration)
        {
            // 알파값 보간
            float targetAlpha = fadingOut ? playerData.fadeAlpha : 1f;
            float currentAlpha = spriteRenderer.color.a;
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, 0.5f);

            spriteRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, newAlpha);

            // 깜빡임 반복
            if (Mathf.Abs(newAlpha - targetAlpha) < 0.05f)
                fadingOut = !fadingOut;

            yield return new WaitForSeconds(playerData.flashInterval);
            elapsed += playerData.flashInterval;
        }

        // 원복
        spriteRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
    }

    private IEnumerator FlashVignette()
    {
        ShaderManager.Instance.SetVignette(0.5f, Color.red);
        yield return new WaitForSeconds(0.2f);
        ShaderManager.Instance.SetVignette();
    }


    #endregion

}