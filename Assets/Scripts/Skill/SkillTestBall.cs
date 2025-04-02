using UnityEngine;

public class SkillTestBall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        int damage = 3;
        // ���� �±׸� ���� ������Ʈ�� �浹���� ��
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject, 0.1f);
        }
    }
}