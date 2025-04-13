using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] public Animator animator;

    enum Direction { Down = 0, Up = 1, Side = 2}

    public void UpdateMovement(Vector2 moveVec)
    {
        float speed = moveVec.magnitude;

        animator.SetFloat("Speed", speed);
    
        if (speed > 0.01f)
        {
            animator.SetBool("Move",true);
            animator.SetFloat("DirectionX", moveVec.x);
            animator.SetFloat("DirectionY", moveVec.y);
            animator.SetInteger("Direction", GetDirectionIndex(moveVec));
        }else{
            animator.SetBool("Move",false);
        }
    }

    public void PlayAttack()
    {
        SetDirection(); // 방향 설정
        SetTrigger("Attack");
    }

    public void PlayDash()
    {
        SetDirection();
        SetTrigger("Dash");
    }

    public void PlayParry()
    {
        SetDirection();
        SetTrigger("Parry");
    }

    public void PlayKnockBack()
    {
        SetDirection();
        SetTrigger("Damaged");
    }

    public void PlayDeath()
    {
        SetDirection();
        SetTrigger("Death");
        animator.SetBool("isDeath", true);
    }

    private void SetTrigger(string triggerName)
    {
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Dash");
        animator.ResetTrigger("Parry");
        animator.ResetTrigger("Damaged");
        animator.ResetTrigger("Death");

        animator.SetTrigger(triggerName);
    }

    private void SetDirection()
    {
        animator.SetInteger("Direction", GetDirectionIndex( PlayerScript.instance.Direction));
    }

    private int GetDirectionIndex(Vector2 vec)
    {
        if (vec.x > 0.5f || vec.x < -0.5f) return (int)Direction.Side;
        else if (vec.y > 0.5f) return (int)Direction.Up;
        else if (vec.y < -0.5f) return (int)Direction.Down;
        else return -1; // Default case, no movement
    }
}
