using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{

    Vector3 moveVec;
    Vector2 lookInput;
    [SerializeField] float speed;
    [SerializeField] float bulletSpeed;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject ammo;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform PlayerModel;


    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + (moveVec * speed * Time.fixedDeltaTime));
    }
    void OnMove(InputValue value)
    {
        moveVec = value.Get<Vector2>().normalized;
    }
    void OnLook(InputValue value)
    {

        //공격의 방향향
        lookInput = value.Get<Vector2>();

        if (lookInput == Vector2.zero)
            return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(lookInput.x, lookInput.y, 0));
        Vector3 direction = (mouseWorldPos - arrow.transform.position).normalized;

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
        if (value.Get<float>() > 0)
        {
            Shoot();
        }
    }
    private void Shoot()
    {
        // 탄환 생성 (현재 화살표 방향 기준)
        GameObject a = Instantiate(ammo, arrow.transform.position, arrow.transform.rotation);

        // Rigidbody2D 추가 후, 발사 방향으로 속도 적용
        Rigidbody2D rb = a.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = arrow.transform.right * bulletSpeed; // 화살표가 바라보는 방향으로 발사
        }

    }

}
