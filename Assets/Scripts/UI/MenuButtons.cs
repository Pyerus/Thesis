using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    public GameObject startButton;
    public GameObject tutorialButton;
    public GameObject quitButton;

    [Header("Dijkstra Choice Buttons")]
    public GameObject standardButton;
    public GameObject modifiedButton;
    public GameObject backButton;

    private void Start()
    {
        ShowMainMenu();
    }

    public void StartGame()
    {
        Debug.Log("Choose Dijkstra Version.");

        
        startButton.SetActive(false);
        tutorialButton.SetActive(false);
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
    }

    public void PlayTutorial()
    {
        Debug.Log("Play Tutorial.");
        SceneManager.LoadScene("TutorialScene");
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
        quitButton.SetActive(true);

        standardButton.SetActive(false);
        modifiedButton.SetActive(false);
        backButton.SetActive(false);
    }
}
