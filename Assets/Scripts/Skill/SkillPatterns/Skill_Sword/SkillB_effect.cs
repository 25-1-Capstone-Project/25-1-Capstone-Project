using UnityEngine;

public class SkillB_effect : MonoBehaviour
{
    // 풀링으로 수정 필요
    void OnEnable()
    {
        Invoke(nameof(Deactivate), 1f);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
