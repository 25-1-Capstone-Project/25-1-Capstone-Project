// 2025.04.29 �ȿ���
// ���̾ �����տ� �پ� ���̾ ��ü�� �ı��� ����� ó���� ����ϴ� ��ũ��Ʈ�Դϴ�.
// 

using UnityEngine;

public class SkillTestBall : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ����������Ʈ �浹�� ��� �߰� ��
        // if (other.CompareTag("Wall") 
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject, 0.1f);
        }
    }
}