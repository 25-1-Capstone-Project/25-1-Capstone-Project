// using UnityEngine;

// public class AnimatorController : MonoBehaviour
// {
//     protected Animator animator;

//     void Awake()
//     {
//         animator = GetComponent<Animator>();
//     }
//     public void SetAnimator(RuntimeAnimatorController animator)
//     {
//         this.animator.runtimeAnimatorController = animator;
//     }
//     public void PlayIdle()
//     {
//         SetTrigger("Idle");
//     }
//     public virtual void PlayAttack(int attackIndex = 0)
//     {
//         SetTrigger("Attack");
//     }
//     public void PlayChase()
//     {
//         SetTrigger("Chase");
//     }
//     public void PlayDeath()
//     {
//         SetTrigger("Death");
//     }
//     public void PlayKnockBack()
//     {
//         SetTrigger("KnockBack");
//     }
//     public void PlayDamaged()
//     {
//         SetTrigger("Damaged");
//     }
//     protected virtual void SetTrigger(string triggerName)
//     {
//         animator.ResetTrigger("Idle");
//         animator.ResetTrigger("Chase");
//         animator.ResetTrigger("Attack");
//         animator.ResetTrigger("Death");
//         animator.ResetTrigger("KnockBack");

//         animator.SetTrigger(triggerName);
//     }
// }