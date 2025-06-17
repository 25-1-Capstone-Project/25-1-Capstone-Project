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
    public Vector2 Direction2D => direction;
    [Header("=====플레이어 상태=====")]
    bool canUseAttack = true;
    bool isParrying = false;
    bool isDead = false;
    bool isAttacking = false;
    bool isDashing = false;
    bool isGod = false; // 무적 상태

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
            if (health > stats.maxHealth)
            {
                health = stats.maxHealth;
            }
            stats.currentHealth = health;

            UIManager.Instance.playerStatUI.UI_HPBarUpdate(stats.currentHealth, stats.maxHealth);
        }
    }

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
    public void SetMaxParryStack(int max) { stats.maxParryStack = max; UIManager.Instance.parryStackUI.SetMaxParryStack(); }

    // 증강
    public event Action OnParrySuccess;
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
        Health = stats.maxHealth;
        SetActivePlayerInput(true);
    }

    #region GetSetFunction
    public PlayerRuntimeStats GetPlayerRuntimeStats() => stats;
    public bool GetIsDead() => isDead;
    public Transform GetPlayerTransform()
    {
        return transform;
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

    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            Health = stats.maxHealth;
        if (Input.GetKeyDown(KeyCode.F2))
            Health = 0;
        if (Input.GetKeyDown(KeyCode.F3))
            ParryStack = stats.maxParryStack;
        if (Input.GetKeyDown(KeyCode.F4))
        {
            EnemyBase[] temp = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
            foreach (EnemyBase enemy in temp)
            {
                enemy.TakeDamage(100);
            }
        }

        PlayerLogger.Instance.AddPlaytimeLog(Time.deltaTime);
    }
    void LateUpdate()
    {
        if (isDead || isAttacking || isParrying || isDashing) return;

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
        if (isDashing || isDead || isParrying)
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
        AudioManager.Instance.PlaySFX("Dash");

        PlayerLogger.Instance.PlusDashLog();
    }

    Vector3 lastSafePosition;

    IEnumerator DashCoroutine()
    {
        //대시 잔상
        ghost.SetSprite(spriteRenderer);
        ghost.SetActive(true);

        gameObject.layer = LayerMask.NameToLayer("PlayerDash"); // 대시 중 플레이어 레이어 변경
        isDashing = true;
        canUseDash = false;

        // 1. 마지막 안전한 위치 저장
        lastSafePosition = transform.position;

        playerAnim.PlayDash(moveVec);
        rb.linearVelocity = moveVec.normalized * (dashDistance / dashDuration);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        ghost.SetActive(false);

        if (IsGroundBelow())
        {
            playerInput.enabled = false;
            isGod = true;
            yield return StartCoroutine(FallAndReturnCoroutine());
            playerInput.enabled = true;
            isGod = false;
            Health -= stats.maxHealth / 12; // 낙하 대미지 처리
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            yield return new WaitForSeconds(dashCooldown);
        }

        canUseDash = true;
        // 대시 후 플레이어 레이어 복원
    }



    public Tilemap fallTilemap;
    public void SetGroundTilemap(Tilemap tilemap)
    {
        fallTilemap = tilemap;
    }
    bool IsGroundBelow()
    {
        if (fallTilemap == null)
            return false;

        Vector3Int cell = fallTilemap.WorldToCell(transform.position);
        return fallTilemap.HasTile(cell);
    }
    IEnumerator FallAndReturnCoroutine()
    {

        float fallTime = 1.0f;
        float shrinkDuration = 0.5f;
        float timer = 0f;

        Vector3 originalScale = transform.localScale;

        // 서서히 작아지며 사라지는 연출
        while (timer < shrinkDuration)
        {
            float t = timer / shrinkDuration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;

        // 잠깐 사라짐
        yield return new WaitForSeconds(fallTime - shrinkDuration);

        // 위치 복구
        transform.position = lastSafePosition;

        // 스케일 원상복구 (순간적으로 or 부드럽게)
        timer = 0f;
        while (timer < 0.3f)
        {
            float t = timer / 0.3f;
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
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
        direction = (mouseWorldPos - transform.position).normalized;
        // 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    #endregion

    #region 공격
    void OnAttack()
    {
        if (!canUseAttack || isDead || isDashing || isParrying || isAttacking)
            return;
        if (!currentSkill.IsCooldownReady())
            return;
        StartCoroutine(AttackRoutine());

        PlayerLogger.Instance.PlusAttackCountLog();
    }
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        canUseAttack = false;
        rb.linearVelocity = Vector2.zero;
        playerAnim.PlayAttack();
        yield return StartCoroutine(currentSkill.CommonSkill(this));

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
        AudioManager.Instance.PlaySFX("Parry");
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

    // 패리 성공|실패 여부에 따라 패리가능 변수 처리, 패리중→패리중X
    public void ParrySuccess(EnemyBase enemy)
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
            //     if (cooldownRoutine != null)
            //     {
            //         StopCoroutine(cooldownRoutine);
            //         cooldownRoutine = null;
            //         UIManager.Instance.skillUI.UpdateCooldown(0f);
            //     }
        }

        OnParrySuccess?.Invoke();

        isParrying = false;
        canUseParry = true;
        enemy.Parried(); // 적에게 대미지 주기
        //enemy?.StateMachine.ChangeState<ParryState>();
        StartCoroutine(ParryEffect());

    }

    public void ParrySuccess(EnemyAttackBase enemyAttack)
    {
        StopCoroutine(ParryRoutine);

        if (ParryStack < stats.maxParryStack)
        {
            ParryStack++;
        }
        // // 주먹구구식으로 땜질해 뒀는데 고찰이 필요...
        // if (ParryStack == stats.maxParryStack)
        // {
        //     currentSkill.ResetCooldown();
        //     if (cooldownRoutine != null)
        //     {
        //         StopCoroutine(cooldownRoutine);
        //         cooldownRoutine = null;
        //         UIManager.Instance.skillUI.UpdateCooldown(0f);
        //     }
        // }

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
        CameraManager.Instance.CameraShake(0.1f, 0.1f);
        EffectPooler.Instance.SpawnFromPool("ParryEffect", transform.position + (direction / 2), Quaternion.identity);
        AudioManager.Instance.PlaySFX("ParrySuccess");
        //isGod = true;
        GameManager.Instance.SetTimeScale(0);
        yield return FadeController.Instance.FadeOut(Color.white, 0.05f, 0.01f);
        yield return FadeController.Instance.FadeIn(Color.white, 0.05f, 0.01f);
        GameManager.Instance.SetTimeScale(1);
        ShaderManager.Instance.CallShockWave();
        yield return new WaitForSeconds(0.1f);
        isGod = false;

    }


    #endregion

    #region 데미지 처리

    // 대미지 처리 함수. 적 스크립트에서 플레이어와 적 충돌 발생 시 호출
    public void TakeDamage(EnemyBase enemy)
    {
        if (isDead) return;
        if (isGod) return;

        if (isParrying)
        {
            float parryDot = Vector2.Dot(direction, -enemy.GetDirectionToPlayerNormalVec());
            float threshold = Mathf.Cos(45f * Mathf.Deg2Rad); // 90도 시야

            if (parryDot >= threshold)
                ParrySuccess(enemy);
            else
            {
                // ParryFailed();
                StartCoroutine(DamagedRoutine(enemy.GetDamage()));
            }
        }
        else
            StartCoroutine(DamagedRoutine(enemy.GetDamage()));


    }
    // 원거리 대미지 처리 함수.

    public void TakeDamage(EnemyAttackBase enemyAttack)
    {
        if (isDead) return;
        if (isGod) return;

        if (isParrying && enemyAttack.CanParry)
        {
            float parryDot = Vector2.Dot(direction, -enemyAttack.GetDirectionNormalVec());
            float threshold = Mathf.Cos(45f * Mathf.Deg2Rad); // 90도 시야

            if (parryDot >= threshold)
                ParrySuccess(enemyAttack);
            else
            {
                if (enemyAttack is ProjectileEnemyAttack)
                {
                    enemyAttack.gameObject.SetActive(false);
                }
                StartCoroutine(DamagedRoutine(enemyAttack.GetDamage()));
            }
        }
        else
        {
            if (enemyAttack is ProjectileEnemyAttack)
            {
                enemyAttack.gameObject.SetActive(false);
            }
            StartCoroutine(DamagedRoutine(enemyAttack.GetDamage()));
        }
    }
    public IEnumerator DamagedRoutine(int damage)
    {
        AudioManager.Instance.PlaySFX("Hit");
        isGod = true;
        playerAnim.PlayKnockBack();
        FlashOnDamage();

        Health -= damage;
        yield return new WaitForSeconds(0.4f);
        isGod = false;

        PlayerLogger.Instance.PlusHitLog();
        PlayerLogger.Instance.AddDamageTakenLog(damage);

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
        // switch (skillType)
        // {
        //     case SkillType.Common:
        //         ParryStack -= currentSkill.commonCost;
        //         currentSkill.SetCooldown();
        //         StartCoroutine(currentSkill.CommonSkill(this));
        //         if (cooldownRoutine != null)
        //             StopCoroutine(cooldownRoutine);
        //         cooldownRoutine = StartCoroutine(CooldownRoutine());
        //         break;
        //     case SkillType.Ultimate:
        //         ParryStack -= currentSkill.ultimateCost;
        //         currentSkill.ResetCooldown();
        //         StartCoroutine(currentSkill.UltimateSkill(this));
        //         break;
        //         //case SkillType.Empty:
        //         //    Debug.Log("스킬 사용 불가");
        //         //    break;
        // }

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
        // AudioManager.Instance.PlaySFX("Ultimate");
        // EffectPooler.Instance.SpawnFromPool("UltimateEffect", PlayerScript.Instance.transform.position, Quaternion.identity);
        CameraManager.Instance.CameraShake(0.1f, 0.1f);
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
        UIManager.Instance.deadInfo.SetActiveDeadInfoPanel(true);
        
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


    public void FlashOnDamage()
    {
        StartCoroutine(FlashRoutine(hitColor));
    }

    private IEnumerator FlashRoutine(Color color)
    {

        spriteRenderer.color = color;


        yield return new WaitForSeconds(flashDuration);


        spriteRenderer.color = Color.white;

    }
    public void FlashParry()
    {
        StartCoroutine(FlashRoutine(Color.blue));
    }


    #endregion

}