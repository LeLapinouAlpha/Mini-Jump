using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAnimations))]
public class PlayerAttacks : MonoBehaviour
{
    [Header("Components references")]
    public PlayerAnimations playerAnimations;

    [Header("Attack actions")]
    public InputAction punchAction;

    [Header("States")]
    private bool isAttacking;
    private bool isPunching;
    private int punchComboCounter;

    void Start()
    {
        this.playerAnimations = this.GetComponent<PlayerAnimations>();

        // FIXME: Create custom input action asset for player attacks
        this.punchAction = InputSystem.actions.FindAction("Punch");
    }

    void UpdatePunch()
    {
        this.isPunching = this.playerAnimations.animator.GetCurrentAnimatorStateInfo(0).IsName("LeftPunching") || this.playerAnimations.animator.GetCurrentAnimatorStateInfo(0).IsName("RightPunching");

        if (this.punchAction.WasPressedThisFrame() && !this.isPunching)
        {
            if (this.punchComboCounter % 2 == 0)
            {
                this.playerAnimations.animator.Play("LeftPunching");
            }
            else
            {
                this.playerAnimations.animator.Play("RightPunching");
            }
            this.punchComboCounter++;
        }
    }

    void Update()
    {
        this.UpdatePunch();

        this.isAttacking = this.isPunching;
    }
}
