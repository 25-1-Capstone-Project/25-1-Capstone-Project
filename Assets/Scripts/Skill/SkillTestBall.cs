using UnityEngine;

public class SkillTestBall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���� �±׸� ���� ������Ʈ�� �浹���� ��
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("����(��ųA)");
            Destroy(gameObject, 0.1f);
        }
    }
}