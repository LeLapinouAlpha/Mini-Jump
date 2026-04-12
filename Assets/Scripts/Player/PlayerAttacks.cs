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
        this.isPunching = this.playerAnimations.IsPlayingAnimation("LeftPunching")
            || this.playerAnimations.IsPlayingAnimation("RightPunching")
            || this.playerAnimations.IsPlayingAnimation("Uppercuting");

        if (this.punchAction.WasPressedThisFrame() && !this.isAttacking)
        {
            if (this.comboCounter == 4)
            {
                this.playerAnimations.PlayUppercutAnimation();
                this.ResetCombo();
                return;
            }

            if (this.comboCounter % 2 == 0)
            {
                this.playerAnimations.PlayLeftPunchAnimation();
            }
            else
            {
                this.playerAnimations.PlayRightPunchAnimation();
            }
            this.IncrementComboCounter();
        }
    }
    private void UpdateKick()
    {
        this.isKicking = this.playerAnimations.IsPlayingAnimation("Kicking");

        if (this.kickAction.WasPressedThisFrame() && !this.isAttacking)
        {
            this.playerAnimations.PlayKickAnimation();
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
