using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    [Header("Starting Money Settings")]
    public float startingMoney = 100000f;

    [Header("UI Reference")]
    public TextMeshProUGUI moneyText;

    private float currentMoney;

    // --- Daily Stats ---
    private float earnedToday = 0f;
    private int itemsSoldToday = 0;

    void Start()
    {
        currentMoney = startingMoney;
        ResetDailyStats(); // Reset stats at game start
        UpdateMoneyUI();
    }

    public bool SpendMoney(float amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }
        else
        {
            Debug.Log("❌ Not enough money!");
            return false;
        }
    }

    // Use this for adding money *without* it counting as a sale (e.g., quest rewards)
    public void AddMoney(float amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    // This is called by NPCs at checkout to log a sale
    public void AddSale(float amount, int itemsSold)
    {
        currentMoney += amount;
        earnedToday += amount;
        itemsSoldToday += itemsSold;
        UpdateMoneyUI();
    }

    // This is called by GameTimer at the start of a new day
    public void ResetDailyStats()
    {
        earnedToday = 0f;
        itemsSoldToday = 0;
    }

    // These are so GameTimer can read the private stats
    public float GetEarnedToday() { return earnedToday; }
    public int GetItemsSoldToday() { return itemsSoldToday; }
    
    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"₱{currentMoney:F2}";
    }

    public float GetCurrentMoney()
    {
        return currentMoney;
    }
}