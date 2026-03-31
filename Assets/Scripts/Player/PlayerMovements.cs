using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls player movement, jumping, sprinting, wall sliding, and wall jumping behaviors in a 2D platformer
/// environment.
/// </summary>
/// <remarks>This component manages physics-based movement and animation states for a player character, including
/// ground and wall interactions. It requires both a Rigidbody2D and PlayerAnimations component to function correctly.
/// Input actions for movement, jumping, and sprinting must be assigned for full functionality. The script handles state
/// updates, collision checks, and animation triggers, and is designed to be used with Unity's update and fixed update
/// cycles. Disabling movement via the canMove property will halt all player actions and reset animations.</remarks>
[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(PlayerAnimations))]
public class PlayerMovements : MonoBehaviour
{
    [Header("Components References")]
    private PlayerAnimations playerAnimations;
    private Rigidbody2D playerRigidbody;

    [Header("Movements Actions")]
    private InputAction walkAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    [Header("General Settings")]
    public bool canMove = true;

    [Header("Ground Jump Settings")]
    public float groundJumpForce = 7f;
    public float defaultGravityScale = 1f;
    public float fallGravityScale = 3f;

    [Header("Walk/Sprint Settings")]
    public float walkSpeed = 3f;
    public float sprintSpeedMultiplier = 2f;

    [Header("Wall Slide/Jump Settings")]
    public float wallJumpingDuration = 0.4f;
    public float wallJumpForceX = 5f;
    public float wallJumpForceY = 7f;
    public float wallSlidingSpeed = 2f;

    [Header("Ground Check")]
    public LayerMask groundLayers;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("Wall Check")]
    public LayerMask wallLayers;
    public Transform wallCheck;
    public float wallCheckRadius = 0.1f;

    [Header("States")]
    private Vector2 movement;
    private bool isGrounded;
    private bool isWalking;
    private bool isSprinting;
    private bool isGroundJumping;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJumping;
    private bool isFalling;
    private Vector2 wallNormal;
    private Vector2 wallJumpForce;
    private float wallJumpingCounter;
    private FacingDirection facingDirection = FacingDirection.Right; // 1 for right, -1 for left

    /*
     * =====================================================================
     *                            INITIALIZATION
     * =====================================================================
     */
    /// <summary>
    /// Initializes references to required components and child transforms attached to the player object.
    /// </summary>
    /// <remarks>This method should be called during the player's setup phase to ensure that all necessary
    /// component and transform references are assigned before gameplay logic depends on them. If any required component
    /// or child transform is missing, related functionality may not work as expected.</remarks>
    private void FindComponents()
    {
        // Get components attached to the player
        this.playerAnimations = this.GetComponent<PlayerAnimations>();
        this.playerRigidbody = this.GetComponent<Rigidbody2D>();

        // Get ground and wall check transforms from child objects
        this.groundCheck = this.transform.Find("GroundCheck");
        this.wallCheck = this.transform.Find("WallCheck");
    }

    /// <summary>
    /// Initializes component values to their default settings.
    /// </summary>
    /// <remarks>This method sets the player's gravity scale to the default value. Call this method to ensure
    /// the player's physics properties are reset before gameplay or after a reset event.</remarks>
    private void InitializeComponents()
    {
        // Set default gravity scale
        this.playerRigidbody.gravityScale = this.defaultGravityScale;
    }

    /// <summary>
    /// Finds and assigns input actions for walking, jumping, and sprinting from the input system.
    /// </summary>
    /// <remarks>This method should be called to initialize the input actions before they are used. If the
    /// required actions are not present in the input system, the corresponding fields will remain unassigned.</remarks>
    private void FindInputActions()
    {
        this.walkAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    /// <summary>
    /// Initializes and prepares the necessary components and input actions for use.
    /// </summary>
    void Start()
    {
        this.FindComponents();
        this.InitializeComponents();
        this.FindInputActions();
    }


    /*
     * =====================================================================
     *                                PHYSICS
     * =====================================================================
     */

    /// <summary>
    /// Checks whether the player is currently grounded and updates the grounded state and related physics properties
    /// accordingly.
    /// </summary>
    /// <remarks>This method should be called regularly, such as during the update loop, to ensure the
    /// player's grounded state and physics behavior remain accurate. When the player is detected as grounded, the
    /// gravity scale is restored to its default value and any vertical velocity is reset to zero.</remarks>
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

    /// <summary>
    /// Checks for the presence of a wall near the player and updates wall contact state and normal vector accordingly.
    /// </summary>
    /// <remarks>This method updates the internal state to indicate whether the player is touching a wall and
    /// sets the wall normal vector based on the closest point of contact. It should be called as part of the
    /// character's physics or movement update cycle to ensure accurate wall detection.</remarks>
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

    /// <summary>
    /// Stops all player movement immediately by resetting movement vectors and velocity.
    /// </summary>
    /// <remarks>Call this method to halt the player's motion, such as when pausing the game or resetting the
    /// player's state. This method affects both the internal movement state and the physics velocity, ensuring the
    /// player comes to a complete stop.</remarks>
    private void StopMovements()
    {
        this.movement = Vector2.zero;
        this.playerRigidbody.linearVelocityX = 0f;
    }

    /// <summary>
    /// Applies an upward force to the player character to perform a ground jump.
    /// </summary>
    /// <remarks>This method should be called when the player initiates a jump from the ground. It resets the
    /// jump state and adjusts gravity to control the jump's trajectory.</remarks>
    private void GroundJump()
    {
        this.playerRigidbody.AddForce(new Vector2(0, this.groundJumpForce), ForceMode2D.Impulse);
        this.playerRigidbody.gravityScale = this.fallGravityScale;
        this.isGroundJumping = false;
    }

    /// <summary>
    /// Limits the player's downward velocity while sliding against a wall to simulate wall sliding behavior.
    /// </summary>
    /// <remarks>This method should be called when the player is in contact with a wall and eligible to slide.
    /// It restricts the vertical speed to prevent the player from falling too quickly while sliding down a wall,
    /// enhancing control and gameplay feel.</remarks>
    private void WallSlide()
    {
        var v = this.playerRigidbody.linearVelocity;
        this.playerRigidbody.linearVelocity = new Vector2(v.x, Mathf.Clamp(v.y, -this.wallSlidingSpeed, float.MaxValue));
    }

    /// <summary>
    /// Performs a wall jump by applying the precomputed wall jump force and adjusting gravity to match the jump arc.
    /// </summary>
    /// <remarks>This method should be called when the player initiates a wall jump action. It sets the
    /// player's velocity and gravity scale to ensure consistent jump behavior. Debug rays are drawn to visualize the
    /// applied forces during development.</remarks>
    private void WallJump()
    {
        // Apply the previously computed wallJumpForce
        this.playerRigidbody.linearVelocity = this.wallJumpForce;

        // Apply fall gravity so arc matches ground jumps
        this.playerRigidbody.gravityScale = this.fallGravityScale;

        // Draw the applied jump vector for debug purposes
        Debug.DrawRay(this.transform.position, (Vector3)this.wallJumpForce, Color.black, 0.2f);
        Debug.DrawRay(this.transform.position, (Vector3)this.wallNormal * 0.5f, Color.yellow, 0.2f);
    }

    /// <summary>
    /// Handles player movement by applying jump forces and updating horizontal velocity based on the player's current
    /// state.
    /// </summary>
    /// <remarks>This method should be called in <code>FixedUpdate()</code> method to ensure responsive movement and jumping behavior. It
    /// manages ground jumps, wall slides, and wall jumps, and prevents horizontal movement during the short lockout period
    /// after a wall jump.</remarks>
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

    /// <summary>
    /// Executes physics-based updates for the player character at fixed time intervals.
    /// </summary>
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

        // Decrease wall jump timer and clear state when finished
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


    /*
     * =====================================================================
     *                         STATES & RENDERING
     * =====================================================================
     */

    /// <summary>
    /// Determines whether the current movement input is directed toward the wall while the character is airborne and
    /// touching a wall.
    /// </summary>
    /// <param name="walkInput">The movement input vector, where the x-component indicates horizontal direction. Typically provided by player
    /// input.</param>
    /// <returns>true if the input is directed into the wall while the character is airborne and touching a wall; otherwise,
    /// false.</returns>
    private bool IsInputTowardWall(Vector2 walkInput)
    {
        // walkInput.x * wallNormal.x < 0 -> input points into the wall
        return this.isTouchingWall && !this.isGrounded && this.isWalking && (walkInput.x * this.wallNormal.x < 0f);
    }

    /// <summary>
    /// Updates the player's walking state and facing direction based on current input and wall jump status.
    /// </summary>
    /// <remarks>Ignores horizontal movement input while the player is wall jumping. Updates animation and
    /// facing direction to reflect the current walking state.</remarks>
    private void UpdateWalk()
    {
        var walkInput = this.walkAction.ReadValue<Vector2>();

        // Ignore horizontal input while wall jumping
        if (this.wallJumpingCounter > 0f)
        {
            return;
        }

        this.movement.x = this.IsInputTowardWall(walkInput) ? 0f : this.walkSpeed * walkInput.x;

        this.isWalking = Mathf.Abs(walkInput.x) > 1e-6f;

        this.playerAnimations.PlayWalkAnimation(this.isWalking);

        this.facingDirection = this.movement.x > 0 ? FacingDirection.Right : (this.movement.x < 0 ? FacingDirection.Left : this.facingDirection);
        this.UpdateFacingDirection();
    }

    /// <summary>
    /// Updates the object's local scale to reflect its current facing direction.
    /// </summary>
    /// <remarks>This method adjusts the x-axis scale of the object's transform to match the value of the
    /// facing direction. It is typically used to visually flip the object in 2D games when changing
    /// direction, but also the relative position of the 'WallCheck' child game object.</remarks>
    private void UpdateFacingDirection()
    {
        var localSale = this.transform.localScale;
        localSale.x = Mathf.Abs(localSale.x) * (int)this.facingDirection;
        this.transform.localScale = localSale;
    }

    /// <summary>
    /// Updates the player's sprinting state and applies sprint-related movement and animation changes.
    /// </summary>
    /// <remarks>This method should be called as part of the player's movement update cycle to ensure that
    /// sprinting behavior and corresponding animations are synchronized with input and movement state.</remarks>
    private void UpdateSprint()
    {
        // Check if player is sprinting and update states with animations
        this.isSprinting = this.sprintAction.IsPressed() && this.isWalking;

        this.movement.x *= this.isSprinting ? this.sprintSpeedMultiplier : 1f;

        this.playerAnimations.PlaySprintAnimation(this.isSprinting);
    }

    /// <summary>
    /// Updates the player's ground jump state and triggers the corresponding jump animation based on input and grounded
    /// status.
    /// </summary>
    /// <remarks>Call this method during the update cycle to ensure the player's jump state and animations
    /// remain in sync with user input and the character's grounded condition.</remarks>
    private void UpdateGroundJump()
    {
        // Check if player is jumping and update states with animations
        this.isGroundJumping = this.jumpAction.IsPressed() && this.isGrounded;

        this.playerAnimations.PlayJumpAnimation(this.isGroundJumping);
    }

    /// <summary>
    /// Updates the player's wall sliding state based on current movement and collision conditions.
    /// </summary>
    /// <remarks>This method should be called during the character's update cycle to ensure the wall sliding
    /// animation and state are synchronized with the player's interactions with walls and the ground.</remarks>
    private void UpdateWallSlide()
    {
        this.isWallSliding = this.isTouchingWall && !this.isGrounded && this.isWalking;
        this.playerAnimations.PlayWallSlideAnimation(this.isWallSliding);
    }

    /// <summary>
    /// Handles the logic for initiating a wall jump when the player is wall sliding and the jump action is pressed.
    /// </summary>
    /// <remarks>This method should be called during the update cycle to detect and process wall jump input.
    /// It sets the appropriate jump force and updates the facing direction based on the wall's position. The actual
    /// jump is typically executed in the physics update after this method sets the relevant state.</remarks>
    private void UpdateWallJump()
    {
        if (this.isWallSliding && this.jumpAction.WasPressedThisFrame())
        {
            // Determine horizontal direction now and store the jump force so FixedUpdate uses the intended direction
            float horizontalDir = (this.wallNormal.x != 0f) ? Mathf.Sign(this.wallNormal.x) : -((int)this.facingDirection);
            this.wallJumpForce.x = this.wallJumpForceX * horizontalDir;
            this.wallJumpForce.y = this.wallJumpForceY;

            this.isWallJumping = true;
            this.wallJumpingCounter = this.wallJumpingDuration;

            // Update facing direction to match the jump direction
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

    /// <summary>
    /// Updates the player's falling state and triggers the appropriate falling animation.
    /// </summary>
    /// <remarks>Call this method to synchronize the player's animation with their current falling status.
    /// This method should be invoked when the player's vertical movement or grounded state changes to ensure accurate
    /// visual feedback.</remarks>
    private void UpdateFall()
    {
        // Check if player is falling and update states with animations
        this.isFalling = this.playerRigidbody.linearVelocity.y < 0 && !this.isGrounded;

        this.playerAnimations.PlayFallingAnimation(this.isFalling);
    }

    /// <summary>
    /// Updates the player's movement and animation states based on the current input and conditions.
    /// </summary>
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


    /*
     * =====================================================================
     *                               EVENTS
     * =====================================================================
     */

    /// <summary>
    /// Draws gizmos in the editor to visualize ground and wall check positions and radii.
    /// </summary>
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
