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

    public void FlipSprite(bool flipX, bool flipY = false)
    {
        this.spriteRenderer.flipX = flipX;
        this.spriteRenderer.flipY = flipY;
    }

    void Start()
    {
        this.animator = this.GetComponent<Animator>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
}
