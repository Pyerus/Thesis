using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    [Header("Starting Money Settings")]
    public float startingMoney = 100000f;

    [Header("UI Reference")]
    public TextMeshProUGUI moneyText;

    private float currentMoney;

    void Start()
    {
        currentMoney = startingMoney;
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

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

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
