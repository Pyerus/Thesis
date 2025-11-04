using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public float maxTimePerDay = 300f;   // 5 minutes per day (in seconds)
    private float currentTime = 0f;
    public TMP_Text timerText;      // Timer display
    public TMP_Text DayText;
    public GameObject nextDayWindow;     // Window that appears when day ends
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

    private readonly string[] daysOfWeek = 
    { 
        "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" 
    };

    void Start()
    {
        if (nextDayWindow != null)
            nextDayWindow.SetActive(false);
        
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
            // Timer advances according to game speed
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
        //Time.timeScale = 0f;
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

        //Time.timeScale = 1f;

        if (npcSpawner != null)
        {
            npcSpawner.spawnLimit = 10;
        }

        MoveLegendToNextDay();
        UpdateDayText();
    }

    private void GameOver()
    {
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
