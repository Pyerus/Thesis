using UnityEngine;
using TMPro;

public class ItemCounter : MonoBehaviour
{
    [Header("Assign all number texts in order")]
    public TextMeshProUGUI[] countTexts; // drag all the "0" texts here
    
    [Header("Link to Game Systems")]
    public Cursors cursor; // Drag your Cursor manager GameObject here
    public ItemObject[] itemsForSale; // MUST BE IN THE SAME ORDER AS TEXTS

    private int[] counts;

    void Start()
    {
        counts = new int[countTexts.Length];
        
        
        if (itemsForSale.Length != countTexts.Length)
        {
            Debug.LogError("ItemCounter Error: The 'itemsForSale' array and 'countTexts' array MUST have the same number of elements.");
        }

        UpdateAllTexts();
    }

    public void Increase(int index)
    {
        if (index >= counts.Length) return;
        counts[index]++;
        UpdateText(index);
    }

    public void Decrease(int index)
    {
        if (index >= counts.Length) return;
        if (counts[index] > 0)
        {
            counts[index]--;
            UpdateText(index);
        }
    }

    // This is the "Order Stock" button
    public void Buy(int index)
    {
        if (index >= counts.Length) return;

        int amountToBuy = counts[index];
        if (amountToBuy <= 0)
        {
            Debug.Log("Select an amount greater than 0 to buy.");
            return;
        }

        // 1. Find the shelf inventory
        if (cursor.GetShelfInventory() == null)
        {
            Debug.LogWarning("Cannot buy: No shelf selected.");
            return;
        }
        InventoryObject shelfInventory = cursor.GetShelfInventory().GetInventoryObject();

        // 2. Get the item data
        ItemObject itemToBuy = itemsForSale[index];
        if (itemToBuy == null)
        {
             Debug.LogError($"ItemCounter Error: No ItemObject assigned at index {index} in 'itemsForSale' array.");
             return;
        }

        // 3. Add item(s) to the shelf inventory
        shelfInventory.AddItem(itemToBuy, amountToBuy);
        Debug.Log($"Stocked {amountToBuy} of {itemToBuy.name} to the shelf.");

        counts[index] = 0;
        UpdateText(index);
    }

    private void UpdateText(int index)
    {
        if (index < countTexts.Length && countTexts[index] != null)
        {
            countTexts[index].text = counts[index].ToString();
        }
    }

    private void UpdateAllTexts()
    {
        for (int i = 0; i < countTexts.Length; i++)
        {
            if (i < counts.Length)
            {
                countTexts[i].text = counts[i].ToString();
            }
        }
    }
}