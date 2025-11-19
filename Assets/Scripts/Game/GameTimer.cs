using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public float maxTimePerDay = 300f;   // 5 minutes per day (in seconds)
    private float currentTime = 0f;
    public TMP_Text timerText;      // Timer display
    public TMP_Text DayText;
    public GameObject nextDayWindow;     // Window that appears when day ends
    public GameObject EndGameWindow;  
    public GameObject Calendar;
    public Button nextDayButton;         // Button inside that window
    public Button CalendarButton; 

    private int currentDay = 1;
    private int maxDays = 7;
    private bool gamePaused = false;
    private bool CalendarOpen = false;

    [SerializeField] private RectTransform legend;  
    private float legendMoveDistance = 12.2f; 
    private Vector2 legendStartPos;

    [SerializeField] private NPCSpawner npcSpawner;

    // --- NEW REFERENCES ---
    [Header("Daily Results UI")]
    public MoneyManager moneyManager;       // Drag your MoneyManager object here
    public TMP_Text itemsSoldText;    // Drag your "ITEMS SOLD:" text here
    public TMP_Text totalEarnedText;  // Drag your "TOTAL EARNED:" text here
    private int totalItemsSoldOverall = 0;
    private float totalEarnedOverall = 0f;

    [Header("End Game Results UI")]
    public TMP_Text finalItemsSoldText;
    public TMP_Text finalTotalEarnedText;
    public Button returnToMenuButton;
    // --- END NEW REFERENCES ---

    private readonly string[] daysOfWeek = 
    { 
        "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" 
    };

    public static GameTimer Instance;
    public int CurrentDay => currentDay;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (nextDayWindow != null)
            nextDayWindow.SetActive(false);

        if (EndGameWindow != null)
            EndGameWindow.SetActive(false);
        
        if (Calendar != null)
            Calendar.SetActive(false);

        if (nextDayButton != null)
            nextDayButton.onClick.AddListener(StartNextDay);

        if (CalendarButton != null)
            CalendarButton.onClick.AddListener(ShowCalendar);

        if (legend != null)
            legendStartPos = legend.anchoredPosition;

        UpdateDayText();
    }
    
    void Update()
    {
        if (!gamePaused)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= maxTimePerDay)
            {
                currentTime = maxTimePerDay;
                if (npcSpawner != null)
                    npcSpawner.spawnLimit = 0;
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
        gamePaused = true;

        if (nextDayWindow != null)
        {
            nextDayWindow.SetActive(true);

            // --- NEW: Populate the results window ---
            if (moneyManager != null)
            {
                itemsSoldText.text = $"ITEMS SOLD: {moneyManager.GetItemsSoldToday()}";
                totalEarnedText.text = $"TOTAL EARNED: ₱{moneyManager.GetEarnedToday():F2}";
                totalItemsSoldOverall += moneyManager.GetItemsSoldToday();
                totalEarnedOverall += moneyManager.GetEarnedToday();
            }
        }
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

        if (npcSpawner != null)
        {
            npcSpawner.CloseStore();
            npcSpawner.spawnLimit = 10;
        }

        // --- NEW: Reset the daily stats for the new day ---
        if (moneyManager != null)
        {
            moneyManager.ResetDailyStats();
        }

        MoveLegendToNextDay();
        UpdateDayText();
    }

    private void GameOver()
    {
        gamePaused = true;
        Time.timeScale = 0f;

        // Hide next-day window if it’s still active
        if (nextDayWindow != null)
            nextDayWindow.SetActive(false);

        // Show final END GAME window
        if (EndGameWindow != null)
        {
            EndGameWindow.SetActive(true);

            // Populate final summary text
            if (finalItemsSoldText != null)
                finalItemsSoldText.text = $"TOTAL ITEMS SOLD: {totalItemsSoldOverall}";

            if (finalTotalEarnedText != null)
                finalTotalEarnedText.text = $"TOTAL EARNED: ₱{totalEarnedOverall:F2}";
        }
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Back to main menu");
        Time.timeScale = 1f;
        CircleTransition.Instance.TransitionToScene("MainMenuScene");
    }

    private void ShowCalendar()
    {
        if (Calendar != null)
        {
            if (CalendarOpen == false)
            {
                Calendar.SetActive(true);
                CalendarOpen = true;
                return;
            }

            else if (CalendarOpen == true)
            {
                Calendar.SetActive(false);
                CalendarOpen = false;
            }
        }
    }

    private void MoveLegendToNextDay()
    {
        if (legend != null)
        {
            Vector2 newPos = legendStartPos + new Vector2(legendMoveDistance * (currentDay - 1), 0);
            legend.anchoredPosition = newPos;
        }
    }

    private void UpdateDayText()
    {
        if (DayText != null && currentDay >= 1 && currentDay <= daysOfWeek.Length)
        {
            DayText.text = daysOfWeek[currentDay - 1];
        }
    }
}