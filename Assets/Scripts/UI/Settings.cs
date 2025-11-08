using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }

    [Header("UI References")]
    public GameObject settingsButton;
    public GameObject settingsPanel;
    public GameObject settingsBackButton;

    private bool isMenuOpen = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        if (settingsBackButton != null)
            settingsBackButton.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuOpen)
            {
                SettingsBack();
                Debug.Log("Close Settings");
            }
            else
            {
                SettingsMenu();
                Debug.Log("Open Settings");
            }
        }
    }

    public void SettingsMenu()
    {
        if (settingsPanel == null || settingsBackButton == null)
        {
            Debug.LogError("Settings UI references are missing in the Inspector!");
            return;
        }

        settingsPanel.SetActive(true);
        settingsBackButton.SetActive(true);
        Time.timeScale = 0f;
        isMenuOpen = true;
    }

    public void SettingsBack()
    {
        if (settingsPanel == null || settingsBackButton == null)
        {
            Debug.LogError("Settings UI references are missing in the Inspector!");
            return;
        }

        settingsPanel.SetActive(false);
        settingsBackButton.SetActive(false);
        Time.timeScale = 1f;
        isMenuOpen = false;
    }
}
