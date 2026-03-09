using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [Header("Components references")]
    public CharacterController characterController;
    public InputAction moveAction;
    public PlayerAnimations playerAnimations;

    [Header("Controls settings")]
    public float movementSpeed = 0.02f;

    void Start()
    {
        // Get components attached to the player
        this.characterController = this.GetComponent<CharacterController>();
        this.playerAnimations = this.GetComponent<PlayerAnimations>();

        // Find input actions
        this.moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        // Read move input
        var movement = this.moveAction.ReadValue<Vector2>();
        if (movement != Vector2.zero)
        {
            // Move the character
            this.characterController.Move(this.movementSpeed * movement.x * Vector2.right);

            // Play walk animation
            this.playerAnimations.PlayWalkAnimation(true);
            this.playerAnimations.FlipSprite(movement.x < 0);
        }
        else
        {
            this.playerAnimations.PlayWalkAnimation(false);
        }
    }
}
