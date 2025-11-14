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

    // --- NEW VARIABLES ---
    private bool isItemMode = true; // true = item list, false = single item
    private ItemObject singleItemToBuy;
    // --- END NEW VARIABLES ---

    void Start()
    {
        counts = new int[countTexts.Length];
        UpdateAllTexts();
    }

    public void Increase(int index)
    {
        // --- MODIFIED ---
        // When you click +/-, switch to item mode
        isItemMode = true;
        singleItemToBuy = null;
        // --- END MODIFIED ---

        if (index >= counts.Length) return;
        counts[index] += incrementAmount;
        UpdateText(index);
        UpdateTotalCost();
    }

    public void Decrease(int index)
    {
        // --- MODIFIED ---
        // When you click +/-, switch to item mode
        isItemMode = true;
        singleItemToBuy = null;
        // --- END MODIFIED ---

        if (index >= counts.Length) return;
        if (counts[index] >= incrementAmount)
            counts[index] -= incrementAmount;
        else
            counts[index] = 0;

        UpdateText(index);
        UpdateTotalCost();
    }

    // --- NEW PUBLIC FUNCTION ---
    // This is what your poster buttons will call
    public void SelectSingleItem(ItemObject itemToBuy)
    {
        isItemMode = false;
        singleItemToBuy = itemToBuy;

        // Clear all item counts so we're not buying both
        for (int i = 0; i < counts.Length; i++)
        {
            if (counts[i] > 0)
            {
                counts[i] = 0;
                UpdateText(i);
            }
        }
        
        UpdateTotalCost();
    }
    // --- END NEW FUNCTION ---

    public void BuyAll()
    {
        // --- MODIFIED ---
        if (isItemMode)
        {
            // This is your original 'BuyAll' logic for items
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
        else
        {
            // This is the new logic for buying a single poster
            if (singleItemToBuy == null)
            {
                Debug.Log("No item selected to buy.");
                return;
            }

            float totalSpent = singleItemToBuy.buyPrice;
            if (moneyManager != null && !moneyManager.SpendMoney(totalSpent))
            {
                Debug.Log("Not enough money to buy this item.");
                return;
            }

            // Add the single item to inventory (assuming 1 poster)
            stockInventory.AddItem(singleItemToBuy, stockInventory.maxCapacity, 1);
            Debug.Log($"Bought {singleItemToBuy.name} for ₱{totalSpent:F2}");

            // Reset
            singleItemToBuy = null;
            isItemMode = true; // Switch back to item mode
            UpdateTotalCost(); // This will reset the total to ₱0
        }
        // --- END MODIFIED ---
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

        // --- MODIFIED ---
        if (isItemMode)
        {
            // Original logic for item list
            for (int i = 0; i < counts.Length; i++)
            {
                if (itemsForSale[i] != null)
                    total += counts[i] * itemsForSale[i].buyPrice;
            }
        }
        else
        {
            // New logic for single selected item
            if (singleItemToBuy != null)
            {
                total = singleItemToBuy.buyPrice;
            }
        }
        // --- END MODIFIED ---

        totalCostText.text = $"Total: ₱{total:F2}";
    }
}