using UnityEngine;


public class EnemyAnimatorController : MonoBehaviour
{
    protected Animator animator;
    public Animator GetAnimator() => animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimator(RuntimeAnimatorController animator)
    {
        this.animator.runtimeAnimatorController = animator;
        SetTrigger("Idle");
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
        SetTrigger("AttackEnd");
    }

    public virtual void PlayAttack()
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
    public void FreezeFrame(bool stop)
    {
        animator.speed = stop ? 0 : 1;

    }
    protected virtual void SetTrigger(string triggerName)
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Chase");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("Damage");
        animator.ResetTrigger("KnockBack");

        animator.SetTrigger(triggerName);
    }
}
