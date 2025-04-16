using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] public Animator animator;

    enum Direction { Down = 0, Up = 1, Right = 2, Left = 3 }

    public void UpdateMovement(Vector2 moveVec)
    {
        float speed = moveVec.magnitude;

        animator.SetFloat("Speed", speed);

        if (speed > 0.01f)
        {
            animator.SetBool("Move", true);
            animator.SetFloat("DirectionX", moveVec.x);
            animator.SetFloat("DirectionY", moveVec.y);
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }
    private void SetDirection(Vector2 dirVec)
    {
        animator.SetFloat("DirectionX", dirVec.x);
        animator.SetFloat("DirectionY", dirVec.y);
        animator.SetInteger("Direction", GetDirectionIndex(dirVec));
    }
    public void PlayAttack()
    {
        SetDirection(PlayerScript.instance.Direction); // 방향 설정
        SetTrigger("Attack");

    }

    public void PlayDash(Vector2 dashVec)
    {
        SetDirection(dashVec);
        SetTrigger("Dash");
    }

    public void PlayParry()
    {
        SetDirection(PlayerScript.instance.Direction);
        SetTrigger("Parry");
    }

    public void PlayKnockBack()
    {
        SetDirection(PlayerScript.instance.Direction);
        SetTrigger("Damaged");
    }

    public void PlayDeath()
    {

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

    private int GetDirectionIndex(Vector2 vec)
    {
        if (Mathf.Abs(vec.x) > Mathf.Abs(vec.y))
        {
            if (vec.x > 0) return (int)Direction.Right;
            else if (vec.x < 0) return (int)Direction.Left;
        }
        else if (Mathf.Abs(vec.x) < Mathf.Abs(vec.y))
        {
            if (vec.y > 0) return (int)Direction.Up;
            else if (vec.y < 0) return (int)Direction.Down;
        }
        return -1; // Default case, no movement
    }
}
