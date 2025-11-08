using UnityEngine;
using TMPro;

public class ItemCounter : MonoBehaviour
{
    [Header("Assign all number texts in order")]
    public TextMeshProUGUI[] countTexts; 
    
    [Header("Link to Game Systems")]
    public Cursors cursor;
    public ItemObject[] itemsForSale; 
    public MoneyManager moneyManager; 

    [Header("Buying Settings")]
    public int incrementAmount = 50; 
    public TextMeshProUGUI totalCostText; 

    private int[] counts;

    void Start()
    {
        counts = new int[countTexts.Length];

        if (itemsForSale.Length != countTexts.Length)
        {
            Debug.LogError("ItemCounter Error: itemsForSale and countTexts length mismatch.");
        }

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

    // ✅ One button for buying all selected items
    public void BuyAll()
    {
        if (cursor.GetShelfInventory() == null)
        {
            Debug.LogWarning("Cannot buy: No shelf selected.");
            return;
        }

        if (moneyManager == null)
        {
            Debug.LogError("MoneyManager is not assigned!");
            return;
        }

        // Calculate total cost
        float totalSpent = 0f;
        for (int i = 0; i < counts.Length; i++)
        {
            if (itemsForSale[i] != null)
                totalSpent += counts[i] * itemsForSale[i].buyPrice;
        }

        
        if (totalSpent > 0f)
        {
            if (!moneyManager.SpendMoney(totalSpent))
            {
                Debug.Log("Not enough money.");
                return;
            }
        }
        else
        {
            Debug.Log("No items selected to buy.");
            return;
        }

        // Proceed with buying
        InventoryObject shelfInventory = cursor.GetShelfInventory().GetInventoryObject();

        int totalItems = 0;
        for (int i = 0; i < counts.Length; i++)
        {
            int amount = counts[i];
            if (amount > 0 && itemsForSale[i] != null)
            {
                shelfInventory.AddItem(itemsForSale[i], amount);
                totalItems += amount;
                counts[i] = 0;
                UpdateText(i);
            }
        }

        UpdateTotalCost();
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
