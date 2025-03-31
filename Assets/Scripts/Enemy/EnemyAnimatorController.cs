using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIdle()
    {
        SetTrigger("Idle");
    }

    public void PlayChase()
    {
        SetTrigger("Chase");
    }

    public void PlayAttack()
    {
        SetTrigger("Attack");
    }

    public void PlayDeath()
    {
        SetTrigger("Death");
    }

    private void SetTrigger(string triggerName)
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Chase");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Death");

        animator.SetTrigger(triggerName);
    }
}
