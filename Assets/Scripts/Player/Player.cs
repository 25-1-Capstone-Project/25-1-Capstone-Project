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

    float angle;
    [SerializeField] bool isParring;
    [SerializeField] bool canUseParry = true;
    Vector3 direction;
    [SerializeField] WaitForSeconds parryDurationSec;
    [SerializeField] WaitForSeconds parryCoolDownSec;
    [SerializeField] PlayerStatus playerStat;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject ammo;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform PlayerModel;
    Coroutine ParryRoutine;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        parryDurationSec = new WaitForSeconds(playerStat.parryDuration);
        parryCoolDownSec = new WaitForSeconds(playerStat.parryCooldown);
    }
    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + (moveVec * playerStat.speed * Time.fixedDeltaTime));
    }
    void OnMove(InputValue value)
    {
        moveVec = value.Get<Vector2>().normalized;
    }
    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();

        if (lookInput == Vector2.zero)
            return;

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
    void OnClick(InputValue value)
    {
        if (!canUseParry)
            return;

        ParryRoutine = StartCoroutine(Parry());
    }

    IEnumerator Parry()
    {
        canUseParry = false;
        isParring = true;
        yield return parryDurationSec;
        ParryFailed();
    }

    public void ParrySucces()
    {
        Debug.Log("패리 성공");
        playerStat.currentParryStack++;
        isParring = false;
        canUseParry = true;
    }
    public void ParryFailed()
    {
        isParring = false;
        canUseParry = false;
        StartCoroutine(ParryCoolDownRoutine());

    }
    IEnumerator ParryCoolDownRoutine()
    {


        yield return parryCoolDownSec;
        canUseParry = true;
        // float coolDown = playerStat.parryCooldown;
        //쿨 도는 거 시각화 필요
        //while(coolDown  >0){}
        yield break;

    }
    public void EnemyContact(Vector3 enemyDirection)
    {
        //데미지 받을때 패리 체크
        //들어온 방향 과 화살표의 방향을 내적
        //-1 ~0 사이일 경우 내 방향 여기에 스트레치를 줘서 반경 조절
        //일치할 경우 데미지 무시
        switch (isParring)
        {
            case true:
                float parryDot = Vector3.Dot(direction, enemyDirection);

                if (parryDot < 0 && parryDot > -1)
                {
                    StopCoroutine(ParryRoutine);
                    ParrySucces();
                }
                else
                {
                    Debug.Log("패리실패");
                    StopCoroutine(ParryRoutine);
                    ParryFailed();
                    Damage();
                }

                break;
            case false:

                Damage();
                break;
        }
    }
    void Damage()
    {
        //데미지 받는 로직
        playerStat.health--;
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }

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
