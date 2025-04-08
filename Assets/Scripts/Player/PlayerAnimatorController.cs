using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] Animator animator;
    enum Direction { Up, Down, Left, Right }
    enum ActionState { Idle, Walk, Attack, Dash, Parry }


    void Update()
    {
        // animator.SetInteger("Direction", Player.instance.GetDirectionIndex());
        //  animator.SetInteger("ActionState", Player.instance.GetActionState());
    }


    public void PlayIdle()
    {
        SetTrigger("Idle");
    }

    public void PlayMove()
    {
        SetTrigger("Move");
    }

    public void PlayAttack()
    {
        SetTrigger("Attack");
    }

    public void PlayDeath()
    {
        SetTrigger("Death");
        animator.SetBool("isDeath", true);
    }

    public void PlayKnockBack()
    {
        SetTrigger("Damaged");
    }

    public void PlayParry()
    {
        SetTrigger("Parry");
    }

    public void PlayDash()
    {
        SetTrigger("Dash");
    }


    private void SetTrigger(string triggerName)
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Move");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("Damaged");
        animator.ResetTrigger("Parry");
        animator.ResetTrigger("Dash");
        animator.SetTrigger(triggerName);
    }
}
