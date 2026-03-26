using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAnimations)), RequireComponent(typeof(PlayerMovements))]
public class PlayerAttacks : MonoBehaviour
{
    [Header("Components References")]
    public PlayerAnimations playerAnimations;
    public PlayerMovements playerMovements;

    [Header("Attack Actions")]
    public InputAction punchAction;

    [Header("Combo Settings")]
    [Tooltip("Time in seconds to reset the combo counter")]
    public float comboResetTime = 1.0f;

    [Header("States")]
    private bool isAttacking;
    private bool isPunching;
    private int comboCounter;
    private float comboTimer = 0f;

    void Start()
    {
        this.playerAnimations = this.GetComponent<PlayerAnimations>();
        this.playerMovements = this.GetComponent<PlayerMovements>();

        // FIXME: Create custom input action asset for player attacks
        this.punchAction = InputSystem.actions.FindAction("Punch");
    }

    private void UpdatePunch()
    {
        this.isPunching = this.playerAnimations.animator.GetCurrentAnimatorStateInfo(0).IsName("LeftPunching") || this.playerAnimations.animator.GetCurrentAnimatorStateInfo(0).IsName("RightPunching");

        if (this.punchAction.WasPressedThisFrame() && !this.isPunching)
        {
            if (this.comboCounter % 2 == 0)
            {
                this.playerAnimations.animator.Play("LeftPunching");
            }
            else
            {
                this.playerAnimations.animator.Play("RightPunching");
            }

            this.IncrementComboCounter();
        }
    }

    private void IncrementComboCounter()
    {

        this.comboCounter++;
        this.comboTimer = 0f;
    }

    private void ResetCombo()
    {
        this.comboTimer = 0f;
        this.comboCounter = 0;
    }

    private void UpdateCombo()
    {
        if (!this.isAttacking)
        {
            this.comboTimer += Time.deltaTime;
            if (this.comboTimer - this.comboResetTime > Mathf.Epsilon)
            {
                this.ResetCombo();
            }
        }
    }

    void Update()
    {
        this.UpdatePunch();

        this.isAttacking = this.isPunching;
        this.playerMovements.canMove = !this.isAttacking;

        this.UpdateCombo();
    }
}
