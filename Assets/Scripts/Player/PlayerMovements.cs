using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [Header("Components references")]
    public PlayerAnimations playerAnimations;
    public Rigidbody2D playerRigidbody;
    public CapsuleCollider2D playerCollider;

    [Header("Movements actions")]
    public InputAction walkAction;
    public InputAction jumpAction;
    public InputAction sprintAction;

    [Header("Controls settings")]
    public float walkSpeed = 3f;
    public float sprintSpeedMultiplier = 2f;
    public float jumpForce = 7f;
    public float defaultGravityScale = 1f;
    public float fallGravityScale = 3f;

    [Header("Ground check")]
    public LayerMask groundLayers;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("States")]
    public Vector2 movement;
    public bool isGrounded;
    public bool isWalking;
    public bool isJumping;
    public bool isFalling;
    public bool isSprinting;

    private void MovePlayer()
    {
        // Move player
        this.playerRigidbody.linearVelocity = new Vector2(this.movement.x, this.playerRigidbody.linearVelocityY);

        // Add jump force if player is jumping
        if (this.isJumping)
        {
            this.playerRigidbody.AddForce(new Vector2(0, this.jumpForce), ForceMode2D.Impulse);
            this.playerRigidbody.gravityScale = this.fallGravityScale;
            this.isJumping = false;
        }
    }

    void Start()
    {
        // Get components attached to the player
        this.playerAnimations = this.GetComponent<PlayerAnimations>();
        this.playerRigidbody = this.GetComponent<Rigidbody2D>();
        this.playerCollider = this.GetComponent<CapsuleCollider2D>();

        // Find input actions
        this.walkAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.sprintAction = InputSystem.actions.FindAction("Sprint");

        // Set default gravity scale
        this.playerRigidbody.gravityScale = this.defaultGravityScale;
    }

    private void FixedUpdate()
    {
        this.isGrounded = Physics2D.OverlapCircle(this.groundCheck.position, this.groundCheckRadius, this.groundLayers);

        this.MovePlayer();
    }

    void Update()
    {
        // Get walk input and update player states with animations
        var walkInput = this.walkAction.ReadValue<Vector2>();
        this.movement.x = this.walkSpeed * walkInput.x;
        this.isWalking = Mathf.Abs(walkInput.x) > 1e-6f;
        this.playerAnimations.PlayWalkAnimation(this.isWalking);
        this.playerAnimations.FlipSprite(walkInput.x < 0);

        // Check if player is jumping and update states with animations
        this.isJumping = this.jumpAction.IsPressed() && this.isGrounded;
        this.playerAnimations.PlayJumpAnimation(this.isJumping);

        // Check if player is grounded and update gravity scale and vertical velocity
        if (this.isGrounded)
        {
            this.playerRigidbody.gravityScale = this.defaultGravityScale;
            this.playerRigidbody.linearVelocity = new Vector2(this.playerRigidbody.linearVelocity.x, 0f);
        }

        // Check if player is falling and update states with animations
        this.isFalling = this.playerRigidbody.linearVelocity.y < 0 && !this.isGrounded;
        this.playerAnimations.PlayFallingAnimation(this.isFalling);

        // Check if player is sprinting and update states with animations
        this.isSprinting = this.sprintAction.IsPressed() && this.isWalking;
        this.movement.x *= this.isSprinting ? this.sprintSpeedMultiplier : 1f;
 
        this.playerAnimations.PlaySprintAnimation(this.isSprinting);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.groundCheck.position, this.groundCheckRadius);
    }
}
