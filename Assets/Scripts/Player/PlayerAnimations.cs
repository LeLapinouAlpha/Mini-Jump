using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [Header("Components references")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public void ResetAnimations()
    {
        this.animator.SetBool("IsWalking", false);
        this.animator.SetBool("IsJumping", false);
        this.animator.SetBool("IsFalling", false);
        this.animator.SetBool("IsSprinting", false);
        this.animator.SetBool("IsWallSliding", false);
    }

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

    public void PlayWallSlideAnimation(bool value)
    {
        this.animator.SetBool("IsWallSliding", value);
    }

    void Start()
    {
        this.animator = this.GetComponent<Animator>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
}
