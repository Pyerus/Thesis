using UnityEngine;
using TMPro;

public class ItemCounter : MonoBehaviour
{
    [Header("Assign all number texts in order")]
    public TextMeshProUGUI[] countTexts; // drag all the "0" texts here
    
    [Header("Link to Game Systems")]
    public MoneyCounter moneyCounter; // update when buying
    public InventoryObject stockInventory; // where the store inventory is placed
    public ItemObject[] itemsForSale; // MUST BE IN SAME ORDER AS TEXTS

    [Header("Buying Settings")]
    public int incrementAmount = 50; // 50 or 100
    public TextMeshProUGUI totalCostText; // optional, shows total ₱

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
        float totalSpent = 0f;
        int totalItems = 0;

        for (int i = 0; i < counts.Length; i++)
        {
            int amount = counts[i];
            if (amount > 0 && itemsForSale[i] != null)
            {
                stockInventory.AddItem(itemsForSale[i], stockInventory.maxCapacity, amount);
                totalSpent += amount * itemsForSale[i].buyPrice;
                totalItems += amount;
                counts[i] = 0;
                UpdateText(i);
            }
        }

        moneyCounter.SubtractMoney(totalSpent);
        UpdateTotalCost();

        if (totalItems > 0)
            Debug.Log($"🛒 Bought {totalItems} items for ₱{totalSpent:F2}");
        else
            Debug.Log("No items selected to buy.");
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
