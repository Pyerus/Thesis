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
        for (int i = 0; i < items.Length; i++)
        {
            int count = stockInventory.GetItemCount(items[i]);
            counterTexts[i].text = count.ToString();
        }
    }
}
