using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [Header("Components references")]
    public InputAction moveAction;
    public InputAction jumpAction;
    public PlayerAnimations playerAnimations;
    public Rigidbody2D playerRigidbody;
    public CapsuleCollider2D playerCollider;

    [Header("Controls settings")]
    public float walkSpeed = 3f;
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
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");

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
        // Get movement input and update player states with animations
        var moveInput = this.moveAction.ReadValue<Vector2>();
        this.movement.x = this.walkSpeed * moveInput.x;
        this.isWalking = Mathf.Abs(moveInput.x) > 1e-6f;
        this.playerAnimations.PlayWalkAnimation(this.isWalking);
        this.playerAnimations.FlipSprite(moveInput.x < 0);

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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.groundCheck.position, this.groundCheckRadius);
    }
}
