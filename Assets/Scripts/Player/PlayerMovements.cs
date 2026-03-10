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
    public float walkSpeed = 1f;
    public float jumpForce = 0.2f;
    public float defaultGravityScale = 3f;
    public float fallGravityScale = 5f;

    [Header("Ground check")]
    public LayerMask groundLayers;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("States")]
    public bool isJumping;
    public bool isGrounded;
    public Vector2 movement;

    private void MovePlayer()
    {
        // Move player
        this.playerRigidbody.linearVelocity = new Vector2(this.movement.x, this.playerRigidbody.linearVelocityY);

        // Add jump force
        if (this.isJumping)
        {
            this.playerRigidbody.AddForce(new Vector2(0, this.jumpForce), ForceMode2D.Impulse);
            this.playerRigidbody.gravityScale = this.fallGravityScale;
            this.isJumping = false;
            this.playerAnimations.PlayJumpAnimation(true);
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

        this.playerRigidbody.gravityScale = this.defaultGravityScale;
    }

    private void FixedUpdate()
    {
        this.isGrounded = Physics2D.OverlapCircle(this.groundCheck.position, this.groundCheckRadius, this.groundLayers);
        this.MovePlayer();
    }

    void Update()
    {
        // Read move input
        var moveInput = this.moveAction.ReadValue<Vector2>();
        this.movement.x = this.walkSpeed * moveInput.x;
        if (moveInput != Vector2.zero)
        {
            // Play walk animation
            this.playerAnimations.PlayWalkAnimation(true);
            this.playerAnimations.FlipSprite(moveInput.x < 0);
        }
        else
        {
            this.playerAnimations.PlayWalkAnimation(false);
        }

        if (this.jumpAction.IsPressed() && this.isGrounded)
        {
            this.isJumping = true;
        }
        else if (this.isGrounded)
        {
            this.playerAnimations.PlayJumpAnimation(false);
            this.playerRigidbody.gravityScale = this.defaultGravityScale;
            this.playerRigidbody.linearVelocity = new Vector2(this.playerRigidbody.linearVelocity.x, 0f);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.groundCheck.position, this.groundCheckRadius);
    }
}
