using UnityEngine;

public class SkillB_effect : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        //���߿� Ǯ������ ����
        Destroy(gameObject);
    }
}
