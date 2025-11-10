using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour
{
    public GameObject settingsButton;
    public GameObject settingsPanel;
    public GameObject settingsBackButton;


    void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        if (settingsBackButton != null)
            settingsBackButton.SetActive(false);
    }

    public void SettingsMenu()
    {
        Debug.Log("Opening Settings Menu.");
        ShowSettingsMenu();
        Time.timeScale = 0f;
    }

    public void SettingsBack()
    {
        Debug.Log("Back from Settings Menu.");
        settingsPanel.SetActive(false);
        settingsBackButton.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ShowSettingsMenu()
    {
        settingsPanel.SetActive(true);
        settingsBackButton.SetActive(true);
    }
}
