using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Components References")]
    private TextMeshProUGUI versionTMP;

    private void DisplayVersion()
    {
        this.versionTMP.text = Application.version.ToString();
    }

    public void Play()
    {
        SceneManager.LoadScene("World01_Level01");
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.versionTMP = this.transform.Find("Version").GetComponent<TextMeshProUGUI>();
        this.DisplayVersion();
    }
}
