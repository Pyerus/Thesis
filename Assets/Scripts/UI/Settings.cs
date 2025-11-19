using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }

    [Header("UI References")]
    public GameObject settingsButton;
    public GameObject settingsPanel;
    public GameObject settingsBackButton;
    public GameObject MainMenuButton;
    public GameObject CreditsButton;
    public GameObject CloseCreditsButton;
    public GameObject creditsImage;

    private bool isMenuOpen = false;
    private bool isCreditsOpen = false;

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
        if (creditsImage != null)
            creditsImage.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isCreditsOpen)
            {
                SettingsBack();
                Debug.Log("Close Settings");
            }
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

    public void ReconnectMainMenuButtons()
    {
        // Find the button again inside the Main Menu scene
        var FindButton = GameObject.Find("SettingsBut"); // exact GameObject name
        if (FindButton == null)
        {
            Debug.LogWarning("Settings Button not found in Main Menu.");
            return;
        }

        settingsButton = FindButton;

        var button = settingsButton.GetComponent<UnityEngine.UI.Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(SettingsMenu);

        Debug.Log("Settings Button in Main Menu reconnected.");
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
        creditsImage.SetActive(false);
        Time.timeScale = 1f;
        isMenuOpen = false;
        isCreditsOpen = false;
    }

    public void BackToMainMenu()
    {
        CircleTransition.Instance.TransitionToScene("MainMenuScene");

        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        isMenuOpen = false;
    }

    public void Credits()
    {
        if (creditsImage != null)
            creditsImage.SetActive(true);
            isCreditsOpen = true;
        Time.timeScale = 0f;
    }

    public void CloseCredits()
    {
        if (creditsImage != null)
            creditsImage.SetActive(false);
            isCreditsOpen = false;
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            ReconnectMainMenuButtons();
        }
    }
}
