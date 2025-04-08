using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] Animator animator;



    public void PlayIdle()
    {
        SetTrigger("Idle");
    }

    public void PlayMove()
    {
        SetTrigger("1_Move");
    }

    public void PlayAttack()
    {
        SetTrigger("2_Attack");
    }

    public void PlayDeath()
    {
        SetTrigger("4_Death");
        animator.SetBool("isDeath", true);
    }

    public void PlayKnockBack()
    {
        SetTrigger("3_Damaged");
    }

    public void PlayParry()
    {
        SetTrigger("7_Parry");
    }

    private void SetTrigger(string triggerName)
    {
        //animator.ResetTrigger("Idle");
        animator.ResetTrigger("1_Move");
        animator.ResetTrigger("2_Attack");
        animator.ResetTrigger("4_Death");
        animator.ResetTrigger("3_Damaged");
        animator.ResetTrigger("7_Parry");

        animator.SetTrigger(triggerName);
    }
}
