using UnityEngine;

public class BossAnimatorController : EnemyAnimatorController
{
    public void StartBattle()
    {
        SetTrigger("endAttack");
    }
     public void PlaySpawn()
    {
        SetTrigger("endAttack");
    }
    public override void PlayAttack(int attackIndex)
    {
        SetTrigger("Attack");
        animator.SetInteger("AttackIndex", attackIndex);
    }
    protected override void SetTrigger(string triggerName)
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Chase");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("KnockBack");
        animator.ResetTrigger("endAttack");
        animator.ResetTrigger("Cooldown");
        animator.ResetTrigger("JumpSmash");
    
        animator.SetTrigger(triggerName);
    }
}
