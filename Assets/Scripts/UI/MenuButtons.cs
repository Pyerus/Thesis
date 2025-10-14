using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuButtons : MonoBehaviour
{
    [Header("Text")]
    public GameObject gameText;

    [Header("Main Menu Buttons")]
    public GameObject startButton;
    public GameObject tutorialButton;
    public GameObject quitButton;
    public GameObject settingsButton;

    [Header("Dijkstra Choice Buttons")]
    public GameObject standardButton;
    public GameObject modifiedButton;
    public GameObject backButton; // existing back for Dijkstra choice

    [Header("Settings Menu")]
    public GameObject settingsPanel;       // assign your small settings panel object here
    public GameObject settingsBackButton;  // new back button for settings

    [Header("Camera Settings")]
    public Camera mainCamera;
    public Transform zoomTarget; // target focus position for settings view
    public float zoomFOV = 25f;
    public float zoomDuration = 1.5f;

    private float originalFOV;
    private Vector3 originalCamPos;
    private bool zoomedIn = false;

    private void Start()
    {
        ShowMainMenu();
        if (mainCamera != null)
        {
            originalCamPos = mainCamera.transform.position;
            originalFOV = mainCamera.fieldOfView;
        }

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        if (settingsBackButton != null)
            settingsBackButton.SetActive(false);
    }

    public void StartGame()
    {
        Debug.Log("Choose Dijkstra Version.");

        startButton.SetActive(false);
        tutorialButton.SetActive(false);
        settingsButton.SetActive(false);
        quitButton.SetActive(false);

        standardButton.SetActive(true);
        modifiedButton.SetActive(true);
        backButton.SetActive(true);
    }

    public void PlayStandardDijkstra()
    {
        Debug.Log("Starting Game with Standard Dijkstra.");
        SceneManager.LoadScene("StandardDijkstra");
    }

    public void PlayModifiedDijkstra()
    {
        Debug.Log("Starting Game with Modified Dijkstra.");
        SceneManager.LoadScene("ModifiedDijkstra");
    }

    public void BackToMainMenu()
    {
        Debug.Log("Returning to Main Menu.");
        ShowMainMenu();

        // reset camera if zoomed in
        if (zoomedIn)
            StartCoroutine(CameraZoom(originalCamPos, originalFOV));
    }

    public void PlayTutorial()
    {
        Debug.Log("Play Tutorial.");
        SceneManager.LoadScene("TutorialScene");
    }

    public void SettingsMenu()
    {
        Debug.Log("Opening Settings Menu.");
        ShowSettingsMenu();

        if (mainCamera != null && zoomTarget != null)
        {
            StartCoroutine(CameraZoom(zoomTarget.position, zoomFOV));
            zoomedIn = true;
        }
    }

    public void SettingsBack()
    {
        Debug.Log("Back from Settings Menu.");
        settingsPanel.SetActive(false);
        settingsBackButton.SetActive(false);
        ShowMainMenu();

        // reset camera zoom
        if (zoomedIn)
            StartCoroutine(CameraZoom(originalCamPos, originalFOV));
        zoomedIn = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game.");
        Application.Quit();
    }

    private void ShowMainMenu()
    {
        startButton.SetActive(true);
        tutorialButton.SetActive(true);
        settingsButton.SetActive(true);
        quitButton.SetActive(true);
        gameText.SetActive(true);

        standardButton.SetActive(false);
        modifiedButton.SetActive(false);
        backButton.SetActive(false);
    }

    private void ShowSettingsMenu()
    {
        startButton.SetActive(false);
        tutorialButton.SetActive(false);
        settingsButton.SetActive(false);
        quitButton.SetActive(false);
        gameText.SetActive(false);

        standardButton.SetActive(false);
        modifiedButton.SetActive(false);
        backButton.SetActive(false);

        settingsPanel.SetActive(true);
        settingsBackButton.SetActive(true);
    }

    private IEnumerator CameraZoom(Vector3 targetPos, float targetFOV)
    {
        Vector3 startPos = mainCamera.transform.position;
        float startFOV = mainCamera.fieldOfView;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            float t = elapsed / zoomDuration;
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.fieldOfView = targetFOV;
    }
}
