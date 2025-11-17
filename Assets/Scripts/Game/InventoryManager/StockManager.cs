using UnityEngine;
using TMPro;

public class StockManager : MonoBehaviour
{
    [Header("References")]
    public InventoryObject stockInventory;

    [Header("Products")] // Should be in order
    public ItemObject[] items;

    [Header("TMP Counters")]
    public TextMeshProUGUI[] counterTexts; // Must match order of items in stockInventory

    private void OnEnable()
    {
        UpdateCounters();
    }

    private void Update()
    {
        UpdateCounters();
    }

    // Public method so other scripts can manually trigger an update
    public void UpdateCounters()
    {
        for (int i = 0; i < counterTexts.Length; i++)
        {
            if (i < stockInventory.Container.Count)
            {
                var slot = stockInventory.Container[i];

                // If the slot has an item, show the count; otherwise, show 0
                string displayText = slot.item != null ? slot.amount.ToString() : "0";

                counterTexts[i].text = displayText;
            }
            else
            {
                // In case there are more text fields than slots
                counterTexts[i].text = "0";
            }
        }
    }
}
