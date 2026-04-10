using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Components References")]
    private GameObject content;
    private GameObject settingsMenu;

    [Header("Input Actions")]
    private InputAction pauseAction;

    public void DisplayContent(bool display)
    {
        this.content.SetActive(display);
        Time.timeScale = display ? 0f : 1f;
    }

    public void Resume()
    {
        this.DisplayContent(false);
    }

    public void Settings()
    {
        this.settingsMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    void Start()
    {
        this.content = this.transform.Find("Content").gameObject;
        this.settingsMenu = this.transform.parent.Find("SettingsMenu").gameObject;
        this.pauseAction = InputSystem.actions.FindAction("Pause");
    }

    void Update()
    {
        if (this.pauseAction.WasPressedThisFrame())
        {
            Debug.Log("Pause button pressed");
            this.DisplayContent(true);
        }
    }
}
