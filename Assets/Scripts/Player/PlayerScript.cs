using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;


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
    bool isKnockback = false;
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
    public void SetMaxParryStack(int max) { stats.maxParryStack = max; UIManager.Instance.parryStackUI.SetMaxParryStack(); }

    // 증강
    public event Action OnParrySuccess;
    public event Action OnParryInput;
    public event Action OnAttackInput;
    private List<PlayerAbility> equipAbilities = new List<PlayerAbility>();

    public void EquipAbility(PlayerAbility ability)
    {
        equipAbilities.Add(ability);
        ability.OnEquip(this);
    }

    public void UnequipAbility(PlayerAbility ability)
    {
        ability.OnUnequip(this);
        equipAbilities.Remove(ability);
    }

    private List<AbilityData> unlockedAbilities = new List<AbilityData>();
    public void RegisterUnlockedAbility(AbilityData data) => unlockedAbilities.Add(data);
    public List<AbilityData> GetUnlockedAbilities() => unlockedAbilities;


    [Header("=====대시 옵션=====")]
    [SerializeField] float dashDistance = 6f;
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
    [SerializeField] PlayerAnimatorController playerAnim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Ghost ghost;

    [SerializeField] private ParticleSystem skillParticle;
    SkillPattern currentSkill;
    private PlayerRuntimeStats stats = new PlayerRuntimeStats();
    public PlayerRuntimeStats Stats => stats;

    public void InitPlayer()
    {
        SetComponent();
        SkillSetting(0);
        stats.ApplyBase(playerData); // 원본 데이터를 복사
        isDead = false;
        playerAnim.SetDeath(isDead);
        playerInput.enabled = true;
        Health = stats.maxHealth;

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
        UIManager.Instance.guideUI.SetActiveGuideUI(true, "[우클릭]!");
        canUseParry = true;
    }
    public void SetCanMove(bool value)
    {
        canMove = value;
    }
    #endregion

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.F1))
        //     Health = stats.maxHealth;
        // if (Input.GetKeyDown(KeyCode.F2))
        //     Health = 0;
        // if (Input.GetKeyDown(KeyCode.F3))
        //     ParryStack = stats.maxParryStack;
        // if (Input.GetKeyDown(KeyCode.F4))
        // {
        //     EnemyBase[] temp = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        //     foreach (EnemyBase enemy in temp)
        //     {
        //         enemy.TakeDamage(100);
        //     }
        // }

        PlayerLogger.Instance.AddPlaytimeLog(Time.deltaTime);
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

        PlayerLogger.Instance.PlusDashLog();
    }


    IEnumerator DashCoroutine()
    {
        //대시 잔상
        ghost.SetSprite(spriteRenderer);
        ghost.SetActive(true);

        gameObject.layer = LayerMask.NameToLayer("PlayerDash"); // 대시 중 플레이어 레이어 변경
        isDashing = true;
        canUseDash = false;

        // 1. 마지막 안전한 위치 저장
        // lastSafePosition = transform.position;

        playerAnim.PlayDash(moveVec);
        rb.linearVelocity = moveVec.normalized * (dashDistance / dashDuration);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        ghost.SetActive(false);

        // if (IsGroundBelow())
        // {
        //     playerInput.enabled = false;
        //     isGod = true;
        //     yield return StartCoroutine(FallAndReturnCoroutine());
        //     playerInput.enabled = true;
        //     isGod = false;
        //     Health -= stats.maxHealth / 12; // 낙하 대미지 처리
        //     gameObject.layer = LayerMask.NameToLayer("Player");
        // }
        // else
        // {
        gameObject.layer = LayerMask.NameToLayer("Player");
        yield return new WaitForSeconds(dashCooldown);
        //}

        canUseDash = true;
    }



    // public Tilemap fallTilemap;
    // public void SetGroundTilemap(Tilemap tilemap)
    // {
    //     fallTilemap = tilemap;
    // }
    // bool IsGroundBelow()
    // {
    //     if (fallTilemap == null)
    //         return false;

    //     Vector3Int cell = fallTilemap.WorldToCell(transform.position);
    //     return fallTilemap.HasTile(cell);
    // }
    // IEnumerator FallAndReturnCoroutine()
    // {

    //     float fallTime = 1.0f;
    //     float shrinkDuration = 0.5f;
    //     float timer = 0f;

    //     Vector3 originalScale = transform.localScale;

    //     // 서서히 작아지며 사라지는 연출
    //     while (timer < shrinkDuration)
    //     {
    //         float t = timer / shrinkDuration;
    //         transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
    //         timer += Time.deltaTime;
    //         yield return null;
    //     }

    //     transform.localScale = Vector3.zero;

    //     // 잠깐 사라짐
    //     yield return new WaitForSeconds(fallTime - shrinkDuration);

    //     // 위치 복구
    //     transform.position = lastSafePosition;

    //     // 스케일 원상복구 (순간적으로 or 부드럽게)
    //     timer = 0f;
    //     while (timer < 0.3f)
    //     {
    //         float t = timer / 0.3f;
    //         transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
    //         timer += Time.deltaTime;
    //         yield return null;
    //     }
    //     transform.localScale = originalScale;
    // }

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
        direction = (mouseWorldPos - transform.position).normalized;
        // 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    #endregion

    #region 공격
    void OnAttack()
    {
        if (isDead || isDashing || isParrying || isAttacking)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            // 클릭 위치를 월드좌표로 변환
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 클릭 위치에서 지름 1(반지름 0.5)의 원 범위 검사
            Collider2D[] hits = Physics2D.OverlapCircleAll(mouseWorld, 0.5f, LayerMask.GetMask("Enemy"));

            if (hits.Length == 0)
                return;

            // 스턴 상태인 적 중 가장 가까운 적 찾기
            EnemyBase closestStunnedEnemy = null;
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
                        closestStunnedEnemy = enemy;
                    }
                }
            }
            Debug.Log(closestStunnedEnemy);
            // 스턴 상태의 적이 있으면 공격 실행
            if (closestStunnedEnemy != null)
            {
                targetEnemy = closestStunnedEnemy;
                OnAttackInput?.Invoke();
                StartCoroutine(AttackRoutine());
            }
        }
    }
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        //canUseAttack = false;
        rb.linearVelocity = Vector2.zero;
        playerAnim.PlayAttack();
        yield return StartCoroutine(AttackEffect());
        isGod = false;
        isAttacking = false;
        targetEnemy = null;
        GameManager.Instance.SetTimeScale(1f);
        CameraManager.Instance.SetLensSize(7f);
    }


    #endregion

    #region 패링
    // 패리 키 입력 받으면 패리 가능여부 확인 후 패리 코루틴 실행
    void OnParry(InputValue value)
    {
        if (!canUseParry || isDead || isAttacking || isDashing)
            return;

        OnParryInput?.Invoke();
        playerAnim.PlayAttack();
        AudioManager.Instance.PlaySFX("Parry");
        ParryRoutine = StartCoroutine(Parry());
    }
    // 패리 코루틴, 일단 패리 사용X, 패리중O 처리→패리지속시간 기다림 뒤 ParryFailed() 호출
    IEnumerator Parry()
    {
        canUseParry = false;
        isParrying = true;
        rb.linearVelocity = Vector2.zero;

        CheckInteractObject();

        // 패리 지속시간이 끝나면 패리중X 처리
        yield return new WaitForSeconds(stats.parryDurationSec);
        isParrying = false;


        // 패리 쿨타임이 끝나면 패리 가능여부 True 처리
        yield return new WaitForSeconds(stats.parryCooldownSec);
        canUseParry = true;
    }

    void CheckInteractObject()
    {
        float checkRange = stats.attackRange;
        float checkAngle = stats.attackAngle;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRange, LayerMask.GetMask("Interactable"));

        foreach (var hit in hits)
        {
            if (hit != null)
            {
                Vector2 toTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector2.Angle(Direction, toTarget);

                if (angle <= checkAngle / 2f)
                {
                    hit.GetComponent<Interactable>().Interact();
                }
            }
        }
    }


    // 근접 패링
    public void ParrySuccess(EnemyBase enemy)
    {
        StopCoroutine(ParryRoutine);
        //PerformParryPulse(enemy);
        if (ParryStack < stats.maxParryStack)
        {
            ParryStack++;
        }
        // if (ParryStack == stats.maxParryStack)
        // {
        //     currentSkill.ResetCooldown();
        // }

        OnParrySuccess?.Invoke();


        canUseParry = true;
        //Health += 1;
        //enemy.TakeDamage(1); // 적에게 대미지 주기
        enemy.Stamina--;
        // if (enemy.Stamina == 0) // 적의 기력 0으로 변경 예정
        // {
        //     AttackStayRoutine = StartCoroutine(AttackStay(enemy));
        // }
        // else
        // {
        StartCoroutine(ParryEffect());
        // }
        isParrying = false;
    }
    //원거리 패링
    public void ParrySuccess(EnemyAttackBase enemyAttack)
    {
        StopCoroutine(ParryRoutine);
        // PerformParryPulse(null);
        if (ParryStack < stats.maxParryStack)
        {
            ParryStack++;

        }

        OnParrySuccess?.Invoke();
        enemyAttack.gameObject.SetActive(true);
        enemyAttack.gameObject.tag = "PlayerAttack";
        enemyAttack.SetDirectionVec(direction); // 방향 반전

        isParrying = false;
        canUseParry = true;
        StartCoroutine(ParryEffect());

    }
    public IEnumerator ParryEffect()
    {
        CameraManager.Instance.CameraShake(3f, 0.2f);
        EffectPooler.Instance.SpawnFromPool("ParryEffect", transform.position + (direction / 2), Quaternion.identity);
        AudioManager.Instance.PlaySFX("Parry" + UnityEngine.Random.Range(0, 3));
        //isGod = true;
        // yield return FadeController.Instance.FadeOut(Color.white, 0.1f, 0.3f);
        ShaderManager.Instance.CallShockWave();

        yield return new WaitForSecondsRealtime(0.1f);
        GameManager.Instance.SetTimeScale(0);
        //   yield return FadeController.Instance.FadeIn(Color.white, 0.1f, 0.3f);
        yield return new WaitForSecondsRealtime(0.1f);
        GameManager.Instance.SetTimeScale(1);

        // yield return new WaitForSeconds(0.1f);
        isGod = false;
    }
    GameObject attackEffect;
    public IEnumerator AttackEffect()
    {
        CameraManager.Instance.CameraShake(8f, 0.3f);
        Vector2 toEnemyDirection = -targetEnemy.GetDirectionNormalVec();
        float angle = Mathf.Atan2(toEnemyDirection.y, toEnemyDirection.x) * Mathf.Rad2Deg;
        RaycastHit2D hit = Physics2D.Raycast(targetEnemy.transform.position, toEnemyDirection, 1.5f, LayerMask.GetMask("Wall"));
        if (hit.collider != null)
        {
            hit.transform.position = (Vector2)hit.transform.position - toEnemyDirection * 0.1f;
        }
        else
        {
            transform.position = (Vector2)targetEnemy.transform.position + toEnemyDirection;
        }


        attackEffect = EffectPooler.Instance.SpawnFromPool("AttackEffect", transform.position, Quaternion.Euler(0, 0, angle));
        AudioManager.Instance.PlaySFX("AttackHit");
        // yield return FadeController.Instance.FadeOut(Color.white, 0.4f, 0.3f);
        ShaderManager.Instance.CallShockWave();
        targetEnemy.TakeDamage(1);
        yield return new WaitForSecondsRealtime(0.4f);
        GameManager.Instance.SetTimeScale(0);
        // yield return FadeController.Instance.FadeIn(Color.white, 0);

        attackEffect.SetActive(false);

        // PerformExecutionKnockback();

        GameManager.Instance.SetTimeScale(1);

        isGod = false;
    }

    //Room 클리어 시 연출 변경 후 정상 작동을 위해 임시로 만든 함수입니다. 빠른 개발 용
    public void ClearSet()
    {
        attackEffect.SetActive(false);
        isGod = false;
        isAttacking = false;
    }

    IEnumerator AttackStay(EnemyBase enemy)
    {
        CameraManager.Instance.SetLensSize(6f);

        isGod = true;
        //canUseAttack = true;
        targetEnemy = enemy;
        EffectPooler.Instance.SpawnFromPool("ParryEffect", transform.position + (direction / 2), Quaternion.identity);
        AudioManager.Instance.PlaySFX("ParrySuccess");
        yield return new WaitForSeconds(0.5f);

        // canUseAttack = false;
        isGod = false;
        CameraManager.Instance.SetLensSize(7f);
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
            float threshold = Mathf.Cos(45f * Mathf.Deg2Rad); // 90도 시야

            if (parryDot >= threshold)
                ParrySuccess(enemy);
            else
            {
                // ParryFailed();
                Debug.Log(1);
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
    [SerializeField] float knockBackForce = 0.5f;
    public void KnockBack(Vector2 forceDir, float knockBackForce)
    {
        //rb.linearVelocity = forceDir * knockBackForce;
        rb.AddForce(forceDir * knockBackForce, ForceMode2D.Impulse);

    }

    public void OnDamaged()
    {
        StartCoroutine(DamagedRoutine(takeAttackDirection));
    }
    public IEnumerator DamagedRoutine(Vector2 forceDir)
    {
        playerInput.enabled = false;
        isGod = true;
        isKnockback = true; // 넉백 시작
        AudioManager.Instance.PlaySFX("Hit");
        playerAnim.PlayDamaged();
        // KnockBack(forceDir, knockBackForce);
        // yield return StartCoroutine(FlashRoutine(hitColor));

        rb.linearVelocity = Vector2.zero;
        isKnockback = false; //넉백 종료
        playerInput.enabled = true;
        yield return StartCoroutine(FlashInvincible());
        isGod = false;
    }



    public void abilTestPlayerHealth(int h)
    {
        Health += h;
    }
    #endregion

    #region 스킬


    // 스킬 셋팅
    public void SkillSetting(int skillNum)
    {
        currentSkill = SkillManager.Instance.SkillPatterns[skillNum];

        if (currentSkill == null)
        {
            SetMaxParryStack(0);
        }
        else
        {
            // 땜질2
            SetMaxParryStack(currentSkill.ultimateCost);
            UIManager.Instance.parryStackUI.SyncParryIcons(ParryStack);
            UIManager.Instance.skillUI.UpdateSkillIcon(currentSkill.skillIcon);
        }
    }


    // 스킬 키 입력
    void OnSkill(InputValue value)
    {
        bool checkUltimate = CheckUltimate();
        if (!checkUltimate)
        {
            return;
        }

        StartCoroutine(UseUltimateSkill());
        PlayerLogger.Instance.PlusSkillUsedLog();
    }

    private bool CheckUltimate()
    {
        if (!currentSkill.ParryStackCheck())
            return false;
        if (currentSkill == null)
            return false;

        if (isDead || isDashing || isParrying)
            return false;

        // 스킬 쿨타임 체크
        if (!currentSkill.IsCooldownReady())
            return false;
        return true;
    }

    private IEnumerator UseUltimateSkill()
    {
        ParryStack -= currentSkill.ultimateCost;
        CameraManager.Instance.CameraShake(2f, 0.1f);
        skillParticle.Play();
        FadeController.Instance.FadeOut(Color.white, 0.05f, 0.01f);
        FadeController.Instance.FadeIn(Color.white, 0.05f, 0.01f);
        GameManager.Instance.SetTimeScale(0.1f);
        yield return new WaitForSecondsRealtime(0.3f);
        GameManager.Instance.SetTimeScale(1f);
        StartCoroutine(currentSkill.UltimateSkill(this));
        ShaderManager.Instance.CallShockWave();
    }

    // IEnumerator CooldownRoutine()
    // {
    //     float duration = currentSkill.cooldown;
    //     float startTime = Time.time;

    //     while (Time.time - startTime < duration)
    //     {
    //         float elapsed = Time.time - startTime;
    //         float ratio = Mathf.Clamp01(1f - (elapsed / duration));
    //         UIManager.Instance.skillUI.UpdateCooldown(ratio);
    //         yield return null;
    //     }

    //     UIManager.Instance.skillUI.UpdateCooldown(0f);
    //     cooldownRoutine = null;
    // }
    #endregion

    public void Dead()
    {
        isDead = true;
        playerAnim.SetDeath(true);
        rb.linearVelocity = Vector2.zero;
        UIManager.Instance.deadUI.SetActiveDeadInfoPanel(true);

        PlayerLogger.Instance.PlusDeathLog();
    }


    #region 인벤토리

    void OnInventory(InputValue value)
    {
        //SkillSetting(1);
        GameManager.Instance.SetTimeScale(0f);
        UIManager.Instance.skillSelect.ShowSkillWindow(OnSkillSelected);
    }

    public void OpenSkillWindow()
    {
        GameManager.Instance.SetTimeScale(0f);
        UIManager.Instance.skillSelect.ShowSkillWindow(OnSkillSelected);
    }

    void OnSkillSelected(int index)
    {
        SkillSetting(index);
    }

    void OnAbilityTest(InputValue value)
    {
        UIManager.Instance.abilityUI.ShowAbilityChoices();
    }

    #endregion


    #region FlashSprite

    public IEnumerator FlashInvincible()
    {
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


    private IEnumerator FlashRoutine(Color color)
    {
        spriteRenderer.color = color;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = Color.white;

    }
    #endregion

}