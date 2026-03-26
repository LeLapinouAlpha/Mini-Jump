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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.textMeshPro = this.GetComponent<TextMeshProUGUI>();

        // Get the PlayerInput component
        this.playerInput = this.GetComponent<PlayerInput>();

        // Get actions from the PlayerInput's action map
        this.punchAction = this.playerInput.actions.FindAction("Punch");
        this.kickAction = this.playerInput.actions.FindAction("Kick");
    }

    // Update is called once per frame
    void Update()
    {
        // GetBindingDisplayString() typically doesn't take a device parameter
        // It shows the binding based on the currently active control scheme
        this.textMeshPro.text =
            $"Punch: {this.punchAction.GetBindingDisplayString()}\n" +
            $"Kick: {this.kickAction.GetBindingDisplayString()}";
    }
}
