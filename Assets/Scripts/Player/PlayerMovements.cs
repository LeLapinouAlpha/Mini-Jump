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
    public bool isWallSliding;
    public float wallSlidingSpeed = 2f;
    private float wallJumpingTime = 0.2f;

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
    public int facingDirection = 1; // 1 for right, -1 for left

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
            var v = this.playerRigidbody.linearVelocity;
            this.playerRigidbody.linearVelocity = new Vector2(v.x, 0f);
        }
    }

    private void WallCheck()
    {
        // Raycast both sides and pick the most relevant hit (or none).
        var leftWallHit = Physics2D.Raycast(this.wallCheckLeft.position, Vector2.left, this.wallCheckRadius, this.wallLayers);
        var rightWallHit = Physics2D.Raycast(this.wallCheckRight.position, Vector2.right, this.wallCheckRadius, this.wallLayers);

        bool leftHit = leftWallHit.collider != null;
        bool rightHit = rightWallHit.collider != null;

        this.isTouchingWall = leftHit || rightHit;

        // If not touching, clear wallNormal only when not in an active wall-jump.
        if (!this.isTouchingWall)
        {
            if (!this.isWallJumping)
            {
                this.wallNormal = Vector2.zero;
            }
            return;
        }

        // If we're currently performing a wall jump, KEEP the cached wallNormal used when the jump started.
        // This prevents the physics raycasts from flipping the normal while we repeatedly apply the jump velocity,
        // which can cause the "stuck / oscillating" behaviour on one side.
        if (this.isWallJumping)
        {
            // We still update isTouchingWall above so other state logic (wall slide) is aware,
            // but we do not overwrite wallNormal during the wall-jump window.
            return;
        }

        // If only one side hits, use its normal. If both hit, use hit.point relative to player center
        if (leftHit && !rightHit)
        {
            this.wallNormal = leftWallHit.normal;
        }
        else if (rightHit && !leftHit)
        {
            this.wallNormal = rightWallHit.normal;
        }
        else // both hit: pick the one whose hit.point is actually on the closest side of the player
        {
            float leftDiff = Mathf.Abs(leftWallHit.point.x - this.transform.position.x);
            float rightDiff = Mathf.Abs(rightWallHit.point.x - this.transform.position.x);

            this.wallNormal = (leftDiff <= rightDiff) ? leftWallHit.normal : rightWallHit.normal;
        }
    }

    // Helper to get an immediate, up-to-date wall normal (used when jump is pressed in Update).
    private Vector2 GetCurrentWallNormal()
    {
        var leftWallHit = Physics2D.Raycast(this.wallCheckLeft.position, Vector2.left, this.wallCheckRadius, this.wallLayers);
        var rightWallHit = Physics2D.Raycast(this.wallCheckRight.position, Vector2.right, this.wallCheckRadius, this.wallLayers);

        bool leftHit = leftWallHit.collider != null;
        bool rightHit = rightWallHit.collider != null;

        if (!leftHit && !rightHit) return Vector2.zero;

        if (leftHit && !rightHit) return leftWallHit.normal;
        if (rightHit && !leftHit) return rightWallHit.normal;

        // both hit: choose based on hit point relative to player center to avoid oscillation when both checks overlap the same surface
        float leftDiff = Mathf.Abs(leftWallHit.point.x - this.transform.position.x);
        float rightDiff = Mathf.Abs(rightWallHit.point.x - this.transform.position.x);

        return (leftDiff <= rightDiff) ? leftWallHit.normal : rightWallHit.normal;
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
        // Compute a direction away from the wall and apply configured forces directly.
        this.wallJumpingDirection = new Vector2(this.wallNormal.x, 1f).normalized;

        float horizontal = (this.wallNormal.x == 0f) ? this.horizontalWallJumpForce : Mathf.Sign(this.wallNormal.x) * this.horizontalWallJumpForce;
        float vertical = this.verticalWallJumpForce;

        this.wallJumpForce = new Vector2(horizontal, vertical);

        // Apply using linearVelocity so input doesn't fight the impulse.
        this.playerRigidbody.linearVelocity = this.wallJumpForce;

        // Apply fall gravity so arc matches ground jumps
        this.playerRigidbody.gravityScale = this.fallGravityScale;

        // Draw the applied jump vector for debug clarity (and draw wall normal)
        Debug.DrawRay(this.transform.position, (Vector3)this.wallJumpForce, Color.black, 0.2f);
        Debug.DrawRay(this.transform.position, (Vector3)(this.wallNormal * 1.0f), Color.yellow, 0.2f);
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
            // do nothing — keep previous movement.x (or set to 0)
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
        if (this.isWalking)
        {
            this.facingDirection = (walkInput.x > 0f) ? 1 : -1;
            this.playerAnimations.FlipSprite(this.facingDirection < 0);
        }
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
    }

    private void UpdateWallJump()
    {
        if (this.isWallSliding && this.jumpAction.WasPressedThisFrame())
        {
            // Get an immediate, up-to-date wall normal at the moment of the jump press and cache it.
            // Caching + preventing WallCheck from overwriting while isWallJumping prevents the flip/oscillation.
            this.wallNormal = this.GetCurrentWallNormal();

            // Optional debug: log values to inspect why a jump might point into the wall
            Debug.Log($"WallJump start - wallNormal: {this.wallNormal} playerX: {this.transform.position.x}");

            // start wall jump: set flag and timer so input is ignored briefly
            this.isWallJumping = true;
            this.wallJumpingCounter = this.wallJumpingDuration;
            // wallJumpForce will be computed and drawn inside WallJump (using the freshly acquired wallNormal)
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.groundCheck.position, this.groundCheckRadius);

        // Draw wall check gizmos as lines
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.wallCheckLeft.position, this.wallCheckLeft.position + Vector3.left * this.wallCheckRadius);
        Gizmos.DrawLine(this.wallCheckRight.position, this.wallCheckRight.position + Vector3.right * this.wallCheckRadius);
    }
}
