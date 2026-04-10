using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Components References")]
    private GameObject content;
    private GameObject settingsMenu;

    [Header("Input Actions")]
    private InputAction pauseAction;

    public void Resume()
    {
        this.content.SetActive(false);
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
            this.content.SetActive(true);
        }
    }
}
