using UnityEngine;

public class SkillB_effect : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        //나중에 풀링으로 변경
        Destroy(gameObject);
    }
}
