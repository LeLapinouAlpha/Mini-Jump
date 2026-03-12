using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [Header("Components references")]
    private PlayerAnimations playerAnimations;
    private Rigidbody2D playerRigidbody;
    private CapsuleCollider2D playerCollider;

    [Header("Movements actions")]
    private InputAction walkAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    [Header("Controls settings")]
    public float walkSpeed = 3f;
    public float sprintSpeedMultiplier = 2f;
    public float jumpForce = 7f;
    public float horizontalWallJumpForce = 5f;
    public float verticalWallJumpForce = 7f;
    public float defaultGravityScale = 1f;
    public float fallGravityScale = 3f;

    [Header("Ground check")]
    public LayerMask groundLayers;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("Wall check")]
    public LayerMask wallLayers;
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckRadius = 0.1f;

    [Header("States")]
    public Vector2 movement;
    public bool isGrounded;
    public bool isWalking;
    public bool isGroundJumping;
    public bool isFalling;
    public bool isSprinting;
    public bool isTouchingWall;
    public Vector2 wallNormal;
    public Vector2 wallJumpDirection;
    public Vector2 wallJumpForce;
    public bool jumpRequested;

    private void FindComponents()
    {
        // Get components attached to the player
        this.playerAnimations = this.GetComponent<PlayerAnimations>();
        this.playerRigidbody = this.GetComponent<Rigidbody2D>();
        this.playerCollider = this.GetComponent<CapsuleCollider2D>();

        // Get ground and wall check transforms from child objects
        this.groundCheck = this.transform.Find("GroundCheck");
        this.wallCheckLeft = this.transform.Find("WallCheckLeft");
        this.wallCheckRight = this.transform.Find("WallCheckRight");
    }

    private void InitializeComponents()
    {
        // Set default gravity scale
        this.playerRigidbody.gravityScale = this.defaultGravityScale;
    }

    private void FindInputActions()
    {
        this.walkAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    private void GroundCheck()
    {
        this.isGrounded = Physics2D.OverlapCircle(this.groundCheck.position, this.groundCheckRadius, this.groundLayers);

        // Check if player is grounded, then restore gravity scale and reset vertical velocity
        if (this.isGrounded)
        {
            this.playerRigidbody.gravityScale = this.defaultGravityScale;
            this.playerRigidbody.linearVelocityY = 0f;
        }
    }

    private void WallCheck()
    {
        var leftWallHit = Physics2D.Raycast(this.wallCheckLeft.position, Vector2.left, this.wallCheckRadius, this.wallLayers);
        var rightWallHit = Physics2D.Raycast(this.wallCheckRight.position, Vector2.right, this.wallCheckRadius, this.wallLayers);
        this.isTouchingWall = rightWallHit || leftWallHit;
        this.wallNormal = leftWallHit ? leftWallHit.normal : (rightWallHit ? rightWallHit.normal : Vector2.zero);
    }

    private void GroundJump()
    {
        this.playerRigidbody.AddForce(new Vector2(0, this.jumpForce), ForceMode2D.Impulse);
        this.playerRigidbody.gravityScale = this.fallGravityScale;
        this.isGroundJumping = false;
    }

    private void WallJump()
    {
        // TODO
    }

    private void MovePlayer()
    {
        // Add jump force if player is jumping
        if (this.isGroundJumping)
        {
            this.GroundJump();
        }

        // Move player
        this.playerRigidbody.linearVelocityX = this.movement.x;
    }

    private void UpdateWalk()
    {
        // Update walk input and states with animations
        var walkInput = this.walkAction.ReadValue<Vector2>();
        this.movement.x = this.walkSpeed * walkInput.x;

        this.isWalking = Mathf.Abs(walkInput.x) > 1e-6f;

        this.playerAnimations.PlayWalkAnimation(this.isWalking);
        this.playerAnimations.FlipSprite(walkInput.x < 0);
    }
    private void UpdateSprint()
    {
        // Check if player is sprinting and update states with animations
        this.isSprinting = this.sprintAction.IsPressed() && this.isWalking;

        this.movement.x *= this.isSprinting ? this.sprintSpeedMultiplier : 1f;

        this.playerAnimations.PlaySprintAnimation(this.isSprinting);
    }

    private void UpdateGroundJump()
    {
        // Check if player is jumping and update states with animations
        this.isGroundJumping = this.jumpAction.IsPressed() && this.isGrounded;

        this.playerAnimations.PlayJumpAnimation(this.isGroundJumping);
    }

    private void UpdateWallJump()
    {
        // TODO
        if (this.isTouchingWall && !this.isGrounded)
        {
            this.wallJumpDirection = new Vector2(this.wallNormal.x, 1).normalized;
            this.wallJumpForce = new Vector2(this.horizontalWallJumpForce * this.wallJumpDirection.x, this.verticalWallJumpForce * this.wallJumpDirection.y);

            // Debug ray cast
            Debug.DrawRay(this.transform.position, this.wallJumpForce, Color.black, 0.1f);
        }
    }

    private void UpdateFall()
    {
        // Check if player is falling and update states with animations
        this.isFalling = this.playerRigidbody.linearVelocity.y < 0 && !this.isGrounded;

        this.playerAnimations.PlayFallingAnimation(this.isFalling);
    }

    void Start()
    {
        // Find components in the player object and its children
        this.FindComponents();

        // Initialize components
        this.InitializeComponents();

        // Find input actions
        this.FindInputActions();
    }

    private void FixedUpdate()
    {
        // Check if player is grounded
        this.GroundCheck();

        // Check if player is touching wall
        this.WallCheck();

        // Move player based on states
        this.MovePlayer();
    }

    void Update()
    {
        // Update horizontal movement and states with animations
        this.UpdateWalk();
        this.UpdateSprint();

        // Update jump states with animations
        this.UpdateGroundJump();
        this.UpdateWallJump();

        // Update falling state with animations
        this.UpdateFall();
    }

    private void OnDrawGizmos()
    {
        // Draw ground check gizmos as a red wire sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.groundCheck.position, this.groundCheckRadius);

        // Draw wall check gizmos as lines
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.wallCheckLeft.position, this.wallCheckLeft.position + Vector3.left * this.wallCheckRadius);
        Gizmos.DrawLine(this.wallCheckRight.position, this.wallCheckRight.position + Vector3.right * this.wallCheckRadius);
    }
}
