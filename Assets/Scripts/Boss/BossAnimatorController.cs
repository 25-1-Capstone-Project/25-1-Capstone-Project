using UnityEngine;

public class BossAnimatorController : EnemyAnimatorController
{
    public void PlayStartBattle()
    {
        SetTrigger("StartBattle");
    }
     public void PlaySpawn()
    {
        SetTrigger("Spawn");
    }
    public override void PlayAttack(int attackIndex)
    {
        SetTrigger("Attack");
        animator.SetInteger("AttackIndex", attackIndex);
    }
    public void SetAttackIndex(int attackIndex)
    {
        animator.SetInteger("AttackIndex", attackIndex);
    }
    protected override void SetTrigger(string triggerName)
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Chase");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("KnockBack");
        animator.ResetTrigger("AttackEnd");


        animator.ResetTrigger("Spawn");

        animator.SetTrigger(triggerName);
    }
}
