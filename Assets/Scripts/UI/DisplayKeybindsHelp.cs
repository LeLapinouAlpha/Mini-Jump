using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshProUGUI)), RequireComponent(typeof(PlayerInput))]
public class DisplayKeybindsHelp : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    public InputAction punchAction;
    public InputAction kickAction;

    private PlayerInput playerInput;

    void Start()
    {
        this.textMeshPro = this.GetComponent<TextMeshProUGUI>();

        this.playerInput = this.GetComponent<PlayerInput>();

        this.punchAction = this.playerInput.actions.FindAction("Punch");
        this.kickAction = this.playerInput.actions.FindAction("Kick");
    }

    void Update()
    {
        this.textMeshPro.text =
            $"Punch: {this.punchAction.GetBindingDisplayString()}\n" +
            $"Kick: {this.kickAction.GetBindingDisplayString()}";
    }
}
