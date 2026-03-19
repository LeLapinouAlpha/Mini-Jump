using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    public enum FacingDirection
    {
        Right = 1,
        Left = -1
    }

    [Header("Components references")]
    private PlayerAnimations playerAnimations;
    private Rigidbody2D playerRigidbody;
    private CapsuleCollider2D playerCollider;

    [Header("Movements actions")]
    private InputAction walkAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    [Header("Controls settings")]
    public bool canMove = true;
    public float walkSpeed = 3f;
    public float sprintSpeedMultiplier = 2f;
    public float jumpForce = 7f;
    public float horizontalWallJumpForce = 5f;
    public float verticalWallJumpForce = 7f;
    public float defaultGravityScale = 1f;
    public float fallGravityScale = 3f;
    public bool isWallSliding;
    public float wallSlidingSpeed = 2f;
    private float wallJumpingTime = 0.2f;

    [Header("Ground check")]
    public LayerMask groundLayers;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("Wall check")]
    public LayerMask wallLayers;
    public Transform wallCheck;
    public float wallCheckRadius = 0.1f;

    [Header("States")]
    public Vector2 movement;
    public bool isGrounded;
    private bool isWalking;
    public bool isGroundJumping;
    public bool isFalling;
    public bool isSprinting;
    public bool isTouchingWall;
    public bool isWallJumping;
    public Vector2 wallNormal;
    public Vector2 wallJumpForce;
    public Vector2 wallJumpingDirection;
    public float wallJumpingCounter;
    public float wallJumpingDuration = 0.4f;
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);
    public FacingDirection facingDirection = FacingDirection.Right; // 1 for right, -1 for left

    private void FindComponents()
    {
        // Get components attached to the player
        this.playerAnimations = this.GetComponent<PlayerAnimations>();
        this.playerRigidbody = this.GetComponent<Rigidbody2D>();
        this.playerCollider = this.GetComponent<CapsuleCollider2D>();

        // Get ground and wall check transforms from child objects
        this.groundCheck = this.transform.Find("GroundCheck");
        this.wallCheck = this.transform.Find("WallCheck");
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
            var v = this.playerRigidbody.linearVelocity;
            this.playerRigidbody.linearVelocity = new Vector2(v.x, 0f);
        }
    }

    private void WallCheck()
    {
        // Find a collider at the wallCheck position
        Collider2D hit = Physics2D.OverlapCircle(this.wallCheck.position, this.wallCheckRadius, this.wallLayers);
        this.isTouchingWall = hit != null;

        if (this.isTouchingWall && hit != null)
        {
            // Compute a reliable wall normal: vector from the closest point on the collider to the player.
            Vector2 closest = hit.ClosestPoint(this.transform.position);
            this.wallNormal = ((Vector2)this.transform.position - closest).normalized;
        }
        else
        {
            this.wallNormal = Vector2.zero;
        }
    }

    private void StopMovements()
    {
        this.movement = Vector2.zero;
        this.playerRigidbody.linearVelocity = Vector2.zero;
    }

    private void GroundJump()
    {
        this.playerRigidbody.AddForce(new Vector2(0, this.jumpForce), ForceMode2D.Impulse);
        this.playerRigidbody.gravityScale = this.fallGravityScale;
        this.isGroundJumping = false;
    }

    private void WallSlide()
    {
        var v = this.playerRigidbody.linearVelocity;
        this.playerRigidbody.linearVelocity = new Vector2(v.x, Mathf.Clamp(v.y, -this.wallSlidingSpeed, float.MaxValue));
    }

    private void WallJump()
    {
        // Apply the previously computed wallJumpForce (so it's consistent even if wallNormal changes)
        this.playerRigidbody.linearVelocity = this.wallJumpForce;

        // Apply fall gravity so arc matches ground jumps
        this.playerRigidbody.gravityScale = this.fallGravityScale;

        // Draw the applied jump vector for debug clarity (and draw wall normal)
        Debug.DrawRay(this.transform.position, (Vector3)this.wallJumpForce, Color.black, 0.2f);
        Debug.DrawRay(this.transform.position, (Vector3)this.wallNormal * 0.5f, Color.yellow, 0.2f);
    }

    private void MovePlayer()
    {
        // Add jump force if player is jumping
        if (this.isGroundJumping)
        {
            this.GroundJump();
        }

        if (this.isWallSliding)
        {
            this.WallSlide();
        }

        if (this.isWallJumping)
        {
            this.WallJump();
        }

        // Move player horizontally only if not in the short wall-jump lockout
        if (!this.isWallJumping && this.wallJumpingCounter <= 0f)
        {
            var v = this.playerRigidbody.linearVelocity;
            this.playerRigidbody.linearVelocity = new Vector2(this.movement.x, v.y);
        }
    }

    private void UpdateWalk()
    {
        var walkInput = this.walkAction.ReadValue<Vector2>();

        // If we are wall-jumping timer active, continue ignoring horizontal input
        if (this.wallJumpingCounter > 0f)
        {
            // do nothing � keep previous movement.x (or set to 0)
            return;
        }

        // If touching a wall and input is directed into the wall, ignore that horizontal input
        bool inputTowardsWall = false;
        if (this.isTouchingWall && !this.isGrounded && Mathf.Abs(walkInput.x) > 1e-6f)
        {
            // walkInput.x * wallNormal.x < 0 -> input points into the wall
            inputTowardsWall = (this.wallNormal.x != 0f) && (walkInput.x * this.wallNormal.x < 0f);
        }

        this.movement.x = inputTowardsWall ? 0f : this.walkSpeed * walkInput.x;

        this.isWalking = Mathf.Abs(walkInput.x) > 1e-6f;

        this.playerAnimations.PlayWalkAnimation(this.isWalking);

        this.facingDirection = this.movement.x > 0 ? FacingDirection.Right : (this.movement.x < 0 ? FacingDirection.Left : this.facingDirection);
        this.UpdateFacingDirection();
    }

    private void UpdateFacingDirection()
    {
        var localSale = this.transform.localScale;
        localSale.x = Mathf.Abs(localSale.x) * (int)this.facingDirection;
        this.transform.localScale = localSale;
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

    private void UpdateWallSlide()
    {
        this.isWallSliding = this.isTouchingWall && !this.isGrounded && this.isWalking;
        this.playerAnimations.PlaySlidingAnimation(this.isWallSliding);
    }

    private void UpdateWallJump()
    {
        if (this.isWallSliding && this.jumpAction.WasPressedThisFrame())
        {
            // Determine horizontal direction now and store the jump force so FixedUpdate uses the intended direction
            float horizontalDir = (this.wallNormal.x != 0f) ? Mathf.Sign(this.wallNormal.x) : -((int)this.facingDirection);
            this.wallJumpForce = new Vector2(this.horizontalWallJumpForce * horizontalDir, this.verticalWallJumpForce);

            this.isWallJumping = true;
            this.wallJumpingCounter = this.wallJumpingDuration;

            // Immediately set facing direction to face away from the wall so the sprite orientation matches the jump.
            if (this.wallNormal.x < 0f) // wall is on the right -> face left
            {
                this.facingDirection = FacingDirection.Left;
            }
            else if (this.wallNormal.x > 0f) // wall is on the left -> face right
            {
                this.facingDirection = FacingDirection.Right;
            }

            this.UpdateFacingDirection();
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
        if (!this.canMove)
        {
            this.StopMovements();
            return;
        }

        // Check if player is grounded
        this.GroundCheck();

        // Check if player is touching wall
        this.WallCheck();

        // Move player based on states
        this.MovePlayer();

        // decrease wall jump timer and clear state when finished
        if (this.wallJumpingCounter > 0f)
        {
            this.wallJumpingCounter -= Time.fixedDeltaTime;
            if (this.wallJumpingCounter <= 0f)
            {
                this.wallJumpingCounter = 0f;
                this.isWallJumping = false;
            }
        }
    }

    void Update()
    {
        if (!this.canMove)
        {
            this.playerAnimations.ResetAnimations();
            return;
        }

        // Update horizontal movement and states with animations
        this.UpdateWalk();
        this.UpdateSprint();

        // Update wall sliding state with animations
        this.UpdateWallSlide();

        // Update jump states with animations
        this.UpdateGroundJump();
        this.UpdateWallJump();

        // Update falling state with animations
        this.UpdateFall();
    }

    private void OnDrawGizmos()
    {
        // Draw ground check gizmos as a red wire sphere
        if (this.groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.groundCheck.position, this.groundCheckRadius);
        }

        // Draw wall check gizmos as lines
        if (this.wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.wallCheck.position, this.wallCheckRadius);
        }
    }
}
