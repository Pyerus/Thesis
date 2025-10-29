using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public float maxTimePerDay = 300f;   // 5 minutes per day (in seconds)
    private float currentTime = 0f;
    public TMP_Text timerText;      // Timer display
    public GameObject nextDayWindow;     // Window that appears when day ends
    public Button nextDayButton;         // Button inside that window

    private int currentDay = 1;
    private int maxDays = 7;
    private bool gamePaused = false;
    private bool gameOver = false;

    void Start()
    {
        // Hide the "Next Day" window initially
        if (nextDayWindow != null)
            nextDayWindow.SetActive(false);

        // Assign button listener
        if (nextDayButton != null)
            nextDayButton.onClick.AddListener(StartNextDay);
    }
    
    void Update()
    {
        if (!gamePaused)
        {
            // Timer advances according to game speed
            currentTime += Time.deltaTime;

            if (currentTime >= maxTimePerDay)
            {
                currentTime = maxTimePerDay;
                EndDay();
            }

            DisplayTime(currentTime);
        }
    }

    void DisplayTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetSpeed(float speed)
    {
        Time.timeScale = speed;  
    }

    void EndDay()
    {
        Time.timeScale = 0f;
        gamePaused = true;

        if (nextDayWindow != null)
            nextDayWindow.SetActive(true);
    }

    public void StartNextDay()
    {
        if (currentDay >= maxDays)
        {
            GameOver();
            return;
        }

        currentDay++;
        currentTime = 0f;
        gamePaused = false;

        if (nextDayWindow != null)
            nextDayWindow.SetActive(false);

        Time.timeScale = 1f;

        //if (npcSpawner != null)
        //    npcSpawner.StartSpawning();
    }

    private void GameOver()
    {
        gameOver = true;
        gamePaused = true;
        Time.timeScale = 0f;

        if (nextDayWindow != null)
        {
            nextDayWindow.SetActive(true);
            TMP_Text text = nextDayWindow.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = "Game Over!\nYou survived all 7 days!";
        }

        if (nextDayButton != null)
            nextDayButton.gameObject.SetActive(false);
    }
}
