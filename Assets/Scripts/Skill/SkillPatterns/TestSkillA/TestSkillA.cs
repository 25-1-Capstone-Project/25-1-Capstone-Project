// 2025.04.29 �ȿ���
// ��ųA(���̾)�� �����ϴ� ��ũ��Ʈ�Դϴ�. (��ų ����: �������� ���̾ �߻� / ���� 3�������� ���� ���̾ �߻�)
// ��� �� ���� ���¿� �´� �Լ��� ȣ��Ǿ�(Common/Ultimate) ���̾ �������� ����, ������ ó���մϴ�
//

using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Player/Skill/SkillA")]
public class TestSkillA : SkillPattern
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed;

    // ���� ®�µ�... Instantiate-Destroy��� ���߿� ���� �� �غ���
    public override IEnumerator CommonSkill(PlayerScript player)
    {

        GameObject fireball = Instantiate(fireballPrefab, player.transform.position, Quaternion.identity, SkillManager.Instance.skillObjectsParent);
        fireball.GetComponent<Rigidbody2D>().linearVelocity = player.Direction * fireballSpeed;

        yield return null;
    }

    public override IEnumerator UltimateSkill(PlayerScript player)
    {
        //player.ParryStack = 0;


        Debug.Log("Ultimate Skill Activated!");
        SpawnFireball(Quaternion.Euler(0, 0, 30f) * player.Direction, player);
        SpawnFireball(Quaternion.Euler(0, 0, -30f) * player.Direction, player);
        SpawnFireball(player.Direction, player);


        yield return null;
    }

    private void SpawnFireball(Vector2 direction, PlayerScript player)
    {
        GameObject fireball = Instantiate(fireballPrefab, player.transform.position, Quaternion.identity, SkillManager.Instance.skillObjectsParent);
        fireball.GetComponent<Rigidbody2D>().linearVelocity = direction * fireballSpeed;

        fireball.GetComponent<SkillTestBall>().damage = damage;
    }

}
