using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [Header("Components references")]
    public CharacterController characterController;
    public InputAction moveAction;

    [Header("Controls settings")]
    public float movementSpeed = 0.02f;

    [Header("States")]
    public bool isMoving;

    void Start()
    {
        // Get components attached to the player
        this.characterController = this.GetComponent<CharacterController>();

        // Find input actions
        this.moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        // Read move input and move the character and update the isMoving state
        var movement = this.moveAction.ReadValue<Vector2>();
        if (movement != Vector2.zero)
        {
            characterController.Move(this.movementSpeed * movement.x * Vector2.right);
            this.isMoving = true;
        }
        else
        {
            this.isMoving = false;
        }
    }
}
