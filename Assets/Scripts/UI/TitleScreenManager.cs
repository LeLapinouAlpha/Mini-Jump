using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Components References")]
    private TextMeshProUGUI versionTMP;
    private TextMeshProUGUI companyTMP;

    private void DisplayVersion()
    {
        this.versionTMP.text = Application.version;
    }

    public void DisplayCompany()
    {
        this.companyTMP.text = Application.companyName;
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
        this.companyTMP = this.transform.Find("Company").GetComponent<TextMeshProUGUI>();
        this.DisplayVersion();
        this.DisplayCompany();
    }
}
