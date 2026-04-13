using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Components References")]
    public Toggle fullscreenToggle;
    public Toggle vsyncToggle;
    public Toggle muteToggle;

    [Header("Debug")]
    public bool debugMode = false;
    public bool showFPS = false;


    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void SetVsync(bool isVsync)
    {
        QualitySettings.vSyncCount = isVsync ? 1 : 0;
    }

    public void SetMute(bool isMuted)
    {
        AudioListener.pause = isMuted;
    }

    public void HideMenu()
    {
        this.gameObject.SetActive(false);
    }

    void Start()
    {
        bool isFullscreen = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
        this.fullscreenToggle.isOn = isFullscreen;

        bool isVsync = QualitySettings.vSyncCount > 0;
        this.vsyncToggle.isOn = isVsync;

        bool isMuted = AudioListener.pause;
        this.muteToggle.isOn = isMuted;
    }

    void OnGUI()
    {
        if (this.debugMode && this.showFPS)
        {
            GUI.Label(new Rect(10, 10, 200, 20), (1.0f / Time.deltaTime).ToString("F0") + " FPS");
        }
    }
}
