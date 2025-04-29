// 2025.04.29 안예선
// 스킬A(파이어볼)를 관리하는 스크립트입니다. (스킬 설명: 전방으로 파이어볼 발사 / 전방 3방향으로 각각 파이어볼 발사)
// 사용 시 현재 상태에 맞는 함수가 호출되어(Common/Ultimate) 파이어볼 프리팹의 생성, 동작을 처리합니다
//

using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Player/Skill/SkillA")]
public class TestSkillA : SkillPattern
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed;

    // 냅다 짰는데... Instantiate-Destroy방식 나중에 생각 좀 해보자
    public override IEnumerator CommonSkill(PlayerScript player){

        GameObject fireball = Instantiate(fireballPrefab, player.transform.position, Quaternion.identity,SkillManager.Instance.skillObjectsParent);
        fireball.GetComponent<Rigidbody2D>().linearVelocity = player.Direction * fireballSpeed;
        
        yield return null;
    }

    public override IEnumerator UltimateSkill(PlayerScript player)
    { 
        //player.ParryStack = 0;
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("Ultimate Skill Activated!");
            SpawnFireball(Quaternion.Euler(0, 0, 30f) * player.Direction, player);
            SpawnFireball(Quaternion.Euler(0, 0, -30f) * player.Direction, player);
            SpawnFireball(player.Direction, player);

            yield return 0.1f;
        }
        yield return null;
    }

    private void SpawnFireball(Vector2 direction, PlayerScript player)
    {
        GameObject fireball = Instantiate(fireballPrefab, player.transform.position, Quaternion.identity, SkillManager.Instance.skillObjectsParent);
        fireball.GetComponent<Rigidbody2D>().linearVelocity = direction * fireballSpeed;

        fireball.GetComponent<SkillTestBall>().damage = damage;
    }

}
