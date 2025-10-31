using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Cursors cursor;
    public InventoryObject inventory;
    public TierManager tierManager;
    

    private void Update()
    {
        if (cursor.GetShelfInventory() != null)
        {
            inventory = cursor.GetShelfInventory().GetInventoryObject();
            tierManager = cursor.GetShelfInventory().GetTierManager();
        }
    }

    public void AddItemToInventory(Item item)
    {
        if (item != null)
        {
            inventory.AddItem(item.item, inventory.maxCapacity);
            tierManager.StockItems(item);
        }
    }
}
