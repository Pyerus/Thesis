using UnityEngine;
using TMPro;

public class ItemCounter : MonoBehaviour
{
    public TextMeshProUGUI[] countTexts;
    public InventoryObject stockInventory;
    public ItemObject[] itemsForSale;
    public MoneyManager moneyManager;

    public int incrementAmount = 50;
    public TextMeshProUGUI totalCostText;

    private int[] counts;

    void Start()
    {
        counts = new int[countTexts.Length];
        UpdateAllTexts();
    }

    public void Increase(int index)
    {
        if (index >= counts.Length) return;
        counts[index] += incrementAmount;
        UpdateText(index);
        UpdateTotalCost();
    }

    public void Decrease(int index)
    {
        if (index >= counts.Length) return;
        if (counts[index] >= incrementAmount)
            counts[index] -= incrementAmount;
        else
            counts[index] = 0;

        UpdateText(index);
        UpdateTotalCost();
    }

    public void BuyAll()
    {
        float totalSpent = 0f;
        int totalItems = 0;

        for (int i = 0; i < counts.Length; i++)
        {
            if (itemsForSale[i] != null)
                totalSpent += counts[i] * itemsForSale[i].buyPrice;
        }

        if (moneyManager != null && !moneyManager.SpendMoney(totalSpent))
        {
            Debug.Log("Not enough money to complete the purchase.");
            return;
        }

        for (int i = 0; i < counts.Length; i++)
        {
            int amount = counts[i];
            if (amount > 0 && itemsForSale[i] != null)
            {
                stockInventory.AddItem(itemsForSale[i], stockInventory.maxCapacity, amount);
                totalItems += amount;
                counts[i] = 0;
                UpdateText(i);
            }
        }

        UpdateTotalCost();

        if (totalItems > 0)
            Debug.Log($"Bought {totalItems} items for ₱{totalSpent:F2}");
    }

    private void UpdateText(int index)
    {
        if (index < countTexts.Length && countTexts[index] != null)
            countTexts[index].text = counts[index].ToString();
    }

    private void UpdateAllTexts()
    {
        for (int i = 0; i < countTexts.Length; i++)
            countTexts[i].text = counts[i].ToString();

        UpdateTotalCost();
    }

    private void UpdateTotalCost()
    {
        if (totalCostText == null) return;

        float total = 0f;
        for (int i = 0; i < counts.Length; i++)
        {
            if (itemsForSale[i] != null)
                total += counts[i] * itemsForSale[i].buyPrice;
        }

        totalCostText.text = $"Total: ₱{total:F2}";
    }
}
