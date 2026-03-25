using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAnimations))]
public class PlayerAttacks : MonoBehaviour
{
    [Header("Components references")]
    public PlayerAnimations playerAnimations;

    [Header("Attack actions")]
    public InputAction punchAction;

    void Start()
    {
        this.playerAnimations = this.GetComponent<PlayerAnimations>();

        // FIXME: Create custom input action asset for player attacks
        this.punchAction = InputSystem.actions.FindAction("Attack");
    }

    void Update()
    {
        if (this.punchAction.WasPressedThisFrame())
        {
            this.playerAnimations.animator.SetTrigger("Punch");
        }
    }
}
