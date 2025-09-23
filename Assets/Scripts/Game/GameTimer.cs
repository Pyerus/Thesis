using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float maxTime = 1800f;   // 30 minutes in seconds
    private float currentTime = 0f;
    public TMP_Text timerText;      // Timer display

    private bool gameEnded = false;

    void Update()
    {
        if (!gameEnded)
        {
            // Timer advances according to game speed
            currentTime += Time.deltaTime;

            if (currentTime >= maxTime)
            {
                currentTime = maxTime;
                PauseGame();
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

    void PauseGame()
    {
        Time.timeScale = 0f;
        gameEnded = true;
        
    }
}
