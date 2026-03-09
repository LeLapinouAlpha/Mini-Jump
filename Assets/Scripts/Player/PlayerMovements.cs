using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [Header("Components references")]
    public CharacterController characterController;
    public InputAction moveAction;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Controls settings")]
    public float movementSpeed = 0.02f;

    void Start()
    {
        // Get components attached to the player
        this.characterController = this.GetComponent<CharacterController>();
        this.animator = this.GetComponent<Animator>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();

        // Find input actions
        this.moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        // Read move input and move the character and update the isMoving state
        var movement = this.moveAction.ReadValue<Vector2>();
        if (movement != Vector2.zero)
        {
            this.characterController.Move(this.movementSpeed * movement.x * Vector2.right);

            this.animator.SetBool("IsWalking", true);

            // Flip the character sprite based on the movement direction
            this.spriteRenderer.flipX = movement.x < 0;
        }
        else
        {
            this.animator.SetBool("IsWalking", false);
        }
    }
}
