using UnityEngine;
using TMPro;

public class ItemCounter : MonoBehaviour
{
    [Header("Shop References")]
    public TextMeshProUGUI[] countTexts;
    public InventoryObject stockInventory;
    public ItemObject[] itemsForSale;
    public MoneyManager moneyManager;
    public TextMeshProUGUI totalCostText;

    [Header("Settings")]
    public int incrementAmount = 50;

    [Header("Purchase Tracking")]
    
    public PurchasedPosters purchasedPosters;
   

    private int[] counts;
    private bool isItemMode = true; 
    private ItemObject singleItemToBuy;

    void Start()
    {
        purchasedPosters = FindFirstObjectByType<PurchasedPosters>();

        counts = new int[countTexts.Length];
        UpdateAllTexts();
    }

    public void Increase(int index)
    {
        isItemMode = true;
        singleItemToBuy = null;

        if (index >= counts.Length) return;
        counts[index] += incrementAmount;
        UpdateText(index);
        UpdateTotalCost();
    }

    public void Decrease(int index)
    {
        isItemMode = true;
        singleItemToBuy = null;

        if (index >= counts.Length) return;
        if (counts[index] >= incrementAmount)
            counts[index] -= incrementAmount;
        else
            counts[index] = 0;

        UpdateText(index);
        UpdateTotalCost();
    }

    public void SelectSingleItem(ItemObject itemToBuy)
    {
        
        if (purchasedPosters.IsPurchased(itemToBuy))
        {
            return;
        }

        isItemMode = false;
        singleItemToBuy = itemToBuy;

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

    public void BuyAll()
    {
        if (isItemMode)
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
        else
        {
            
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

            stockInventory.AddItem(singleItemToBuy, stockInventory.maxCapacity, 1);
            Debug.Log($"Bought {singleItemToBuy.name} for ₱{totalSpent:F2}");

           
            
            purchasedPosters.MarkAsPurchased(singleItemToBuy);
           

            singleItemToBuy = null;
            isItemMode = true; 
            UpdateTotalCost(); 
        }
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

        if (isItemMode)
        {
            for (int i = 0; i < counts.Length; i++)
            {
                if (itemsForSale[i] != null)
                    total += counts[i] * itemsForSale[i].buyPrice;
            }
        }
        else
        {
            if (singleItemToBuy != null)
            {
                total = singleItemToBuy.buyPrice;
            }
        }

        totalCostText.text = $"Total: ₱{total:F2}";
    }
}