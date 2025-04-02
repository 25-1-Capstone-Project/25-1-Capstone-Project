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


    [SerializeField] bool isParring;
    [SerializeField] bool canUseParry = true;
    [SerializeField] bool canUseAttack = true;

    [SerializeField] WaitForSeconds attackCoolDownSec;
    [SerializeField] WaitForSeconds parryDurationSec;
    [SerializeField] WaitForSeconds parryCoolDownSec;
    [SerializeField] PlayerStatus playerStat;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject ammo;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform PlayerModel;
    [SerializeField] ParticleSystem attackSlashParticle;
    SkillPattern currentSkill;

    Vector3 direction;
    public Vector3 Direction => direction;
    Coroutine ParryRoutine;


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

    public int ParryStack
    {
        get { return playerStat.currentParryStack; }
        set { playerStat.currentParryStack = Mathf.Max(value, 0); }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 플레이어 스탯 기반으로 패리 시간·쿨타임 설정
        parryDurationSec = new WaitForSeconds(playerStat.parryDuration);
        parryCoolDownSec = new WaitForSeconds(playerStat.parryCooldown);

        currentSkill = SkillManager.instance.SkillPatterns[0];
    }
    void InitPlayer()
    {
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
        moveVec = value.Get<Vector2>().normalized;
    }
    #endregion

    #region 방향
    // 마우스 이동으로 보는 방향 처리(마우스 위치값(OnLook)→Look()호출, 캐릭터 보는 방향 조절)
    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();

        if (lookInput == Vector2.zero)
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
    // 플레이어인풋으로 클릭 받아서 현재 공격 사용 가능할 경우 코루틴 돌림
    void OnAttack(InputValue value)
    {
        if (!canUseAttack)
            return;

        StartCoroutine(AttackRoutine());
    }
    IEnumerator AttackRoutine()
    {
        canUseAttack = false;
        attackSlashParticle.Play();

        yield return attackCoolDownSec;
        canUseAttack = true;
    }
    #endregion
    #region 패링
    // 패리 키 입력 받으면 패리 가능여부 확인 후 패리 코루틴 실행
    void OnParry(InputValue value)
    {
        if (!canUseParry)
            return;

        ParryRoutine = StartCoroutine(Parry());
    }
    // 패리 코루틴, 일단 패리 사용X, 패리중O 처리→패리지속시간 기다림 뒤 ParryFailed() 호출
    IEnumerator Parry()
    {
        canUseParry = false;
        isParring = true;
        yield return parryDurationSec;
        ParryFailed();
    }

    // 패리 성공|실패 여부에 따라 패리가능 변수 처리, 패리중→패리중X, 패리 실패 시 쿨다운 코루틴 호출
    public void ParrySucces(Enemy enemy, Vector2 enemyDirection)
    {
        playerStat.currentParryStack++;
        isParring = false;
        canUseParry = true;

        enemy.KnockBack(enemyDirection);
    }
    public void ParryFailed()
    {
        Debug.Log("패리 실패");
        isParring = false;
        canUseParry = false;
        StartCoroutine(ParryCoolDownRoutine());

    }
    // 패리 쿨다운 코루틴, 패리 쿨만큼 기다렸다가 패리가능여부 True;
    IEnumerator ParryCoolDownRoutine()
    {
        yield return parryCoolDownSec;
        canUseParry = true;
        // float coolDown = playerStat.parryCooldown;
        //쿨 도는 거 시각화 필요
        //while(coolDown  >0){}
        yield break;

    }
    #endregion

    // 대미지 처리 함수. 적 스크립트에서 플레이어와 적 충돌 발생 시 호출
    public void TakeDamage(int damage, Vector2 enemyDirection, Enemy enemy)
    {
        //데미지 받을때 패리 체크
        //들어온 방향 과 화살표의 방향을 내적
        //-1 ~0 사이일 경우 내 방향 여기에 스트레치를 줘서 반경 조절
        //일치할 경우 데미지 무시
        switch (isParring)
        {
            case true:
                float parryDot = Vector2.Dot(direction, enemyDirection);

                if (parryDot < 0 && parryDot > -1)
                {
                    if (ParryRoutine != null)
                        StopCoroutine(ParryRoutine);

                    ParrySucces(enemy, direction);
                }
                else
                {
                    Debug.Log("패리실패");
                    if (ParryRoutine != null)
                        StopCoroutine(ParryRoutine);

                    ParryFailed();
                    StartCoroutine(DamagedRoutine(damage));
                }
                break;
            case false:
                StartCoroutine(DamagedRoutine(damage));
                break;
        }
    }
    public IEnumerator DamagedRoutine(int damage)
    {
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


}
