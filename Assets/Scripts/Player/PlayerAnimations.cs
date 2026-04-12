using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
    [Header("Components References")]
    private Animator animator;

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

    public void PlayLeftPunchAnimation()
    {
        this.animator.Play("LeftPunching");
    }

    public void PlayRightPunchAnimation()
    {
        this.animator.Play("RightPunching");
    }

    public void PlayUppercutAnimation()
    {
        this.animator.Play("Uppercuting");
    }

    public void PlayKickAnimation()
    {
        this.animator.Play("Kicking");
    }

    public bool IsPlayingAnimation(string animationName)
    {
        return this.animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    void Start()
    {
        this.animator = this.GetComponent<Animator>();
    }
}
