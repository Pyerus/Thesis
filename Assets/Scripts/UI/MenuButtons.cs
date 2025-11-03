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
    public GameObject backButton; // for Dijkstra choices

    [Header("Settings Menu")]
    public GameObject settingsPanel;
    public GameObject settingsBackButton; // for settings panel only

    [Header("Camera Settings")]
    public Camera mainCamera;
    public Transform zoomTargetLeft;  // ← add this (left side of cat)
    public Transform zoomTargetRight; // ← existing one (right side of cat)
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

        // Zoom in on left side of cat
        if (mainCamera != null && zoomTargetLeft != null)
        {
            StartCoroutine(CameraZoom(zoomTargetLeft.position, zoomFOV));
            zoomedIn = true;
        }
        standardButton.SetActive(true);
        modifiedButton.SetActive(true);
        backButton.SetActive(true);

    }



    public void PlayStandardDijkstra()
    {
        Debug.Log("Starting Game with Standard Dijkstra.");
        CircleTransition.Instance.TransitionToScene("StandardDijkstra");
    }

    public void PlayModifiedDijkstra()
    {
        Debug.Log("Starting Game with Modified Dijkstra.");
        CircleTransition.Instance.TransitionToScene("ModifiedDijkstra");
    }

    public void BackToMainMenu()
    {
        Debug.Log("Returning to Main Menu.");
        ShowMainMenu();

        if (zoomedIn)
            StartCoroutine(CameraZoom(originalCamPos, originalFOV));
        zoomedIn = false;
    }

    public void PlayTutorial()
    {
        Debug.Log("Play Tutorial.");
        CircleTransition.Instance.TransitionToScene("TutorialScene");
    }


    public void SettingsMenu()
    {
        Debug.Log("Opening Settings Menu.");
        ShowSettingsMenu();

        if (mainCamera != null && zoomTargetRight != null)
        {
            StartCoroutine(CameraZoom(zoomTargetRight.position, zoomFOV));
            zoomedIn = true;
        }
    }

    public void SettingsBack()
    {
        Debug.Log("Back from Settings Menu.");
        settingsPanel.SetActive(false);
        settingsBackButton.SetActive(false);
        ShowMainMenu();

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
            t = Mathf.SmoothStep(0, 1, t); // smooth easing
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.fieldOfView = targetFOV;
    }
}
