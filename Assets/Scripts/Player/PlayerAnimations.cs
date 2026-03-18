using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [Header("Components references")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public void PlayWalkAnimation(bool value)
    {
        this.animator.SetBool("IsWalking", value);
    }

    public void PlayJumpAnimation(bool value)
    {
        this.animator.SetBool("IsJumping", value);
    }

    public void PlayFallingAnimation(bool value)
    {
        this.animator.SetBool("IsFalling", value);
    }

    public void PlaySprintAnimation(bool value)
    {
        this.animator.SetBool("IsSprinting", value);
    }

    public void PlaySlidingAnimation(bool value)
    {
        this.animator.SetBool("IsSliding", value);
    }

    void Start()
    {
        this.animator = this.GetComponent<Animator>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
}
