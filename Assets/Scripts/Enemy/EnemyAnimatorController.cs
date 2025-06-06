using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    protected Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimator(RuntimeAnimatorController animator)
    {
        this.animator.runtimeAnimatorController = animator;
    }
    public void PlayIdle()
    {
        SetTrigger("Idle");
    }

    public void PlayChase()
    {
        SetTrigger("Chase");
    }
    public void PlayCooldown()
    {
        SetTrigger("Cooldown");
    }
    public void EndAttack()
    {
        SetTrigger("endAttack");
    }

    public virtual void PlayAttack(int attackIndex=0)
    {
        SetTrigger("Attack");
    }
    public void PlayDamage()
    {
        SetTrigger("Damage");
    }
    public void PlayDeath()
    {
        SetTrigger("Death");
    }

    public void PlayKnockBack()
    {
        SetTrigger("KnockBack");
    }

    protected virtual void SetTrigger(string triggerName)
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Chase");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("Damage");

        animator.SetTrigger(triggerName);
    }
}
