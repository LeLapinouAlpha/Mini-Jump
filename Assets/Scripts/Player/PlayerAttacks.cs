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
    public InputAction kickAction;

    [Header("Combo Settings")]
    [Tooltip("Time in seconds to reset the combo counter")]
    public float comboResetTime = 1.0f;

    [Header("States")]
    private bool isAttacking;
    private bool isPunching;
    private bool isKicking;
    private int comboCounter;
    private float comboTimer = 0f;

    void Start()
    {
        this.playerAnimations = this.GetComponent<PlayerAnimations>();
        this.playerMovements = this.GetComponent<PlayerMovements>();

        this.punchAction = InputSystem.actions.FindAction("Punch");
        this.kickAction = InputSystem.actions.FindAction("Kick");
    }

    private void UpdatePunch()
    {
        this.isPunching = this.playerAnimations.animator.GetCurrentAnimatorStateInfo(0).IsName("LeftPunching")
            || this.playerAnimations.animator.GetCurrentAnimatorStateInfo(0).IsName("RightPunching")
            || this.playerAnimations.animator.GetCurrentAnimatorStateInfo(0).IsName("Uppercuting");

        if (this.punchAction.WasPressedThisFrame() && !this.isPunching)
        {
            if (this.comboCounter == 4)
            {
                this.playerAnimations.animator.Play("Uppercuting");
                this.ResetCombo();
                return;
            }

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
    private void UpdateKick()
    {
        this.isKicking = this.playerAnimations.animator.GetCurrentAnimatorStateInfo(0).IsName("Kicking");

        if (this.kickAction.WasPressedThisFrame() && !this.isKicking)
        {
            this.playerAnimations.animator.Play("Kicking");
            this.ResetCombo();
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
        this.UpdateKick();

        this.isAttacking = this.isPunching || this.isKicking;
        this.playerMovements.canMove = !this.isAttacking;

        this.UpdateCombo();
    }
}
