using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    Animator animator;

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

    public void PlayKnockBack()
    {
        SetTrigger("KnockBack");
    }

    private void SetTrigger(string triggerName)
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Chase");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("KnockBack");

        animator.SetTrigger(triggerName);
    }
}
