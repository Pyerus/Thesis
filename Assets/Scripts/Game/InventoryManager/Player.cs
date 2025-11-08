using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Cursors cursor;
    public InventoryObject stockInventory;
    public InventoryObject shelfInventory;
    public TierManager tierManager;
    

    private void Update()
    {
        if (cursor.GetShelfInventory() != null)
        {
            shelfInventory = cursor.GetShelfInventory().GetInventoryObject();
            tierManager = cursor.GetShelfInventory().GetTierManager();
        }
    }

    public void AddItemToInventory(Item item)
    {
        if (item != null)
        {
            int addAmount = 0;
            
            // get the current amount in stock
            int stock = stockInventory.GetItemCount(item.item);
            int max = tierManager.GetMaxAmount(item);

            // if there are less than the maximum capacity in stock, add only the amount available
            if (stock < max)
                addAmount = stock;
            else
                addAmount = max;

            var added = shelfInventory.AddItem(item.item, max, addAmount);
            tierManager.StockItems(item, added.slotIndex, added.amountAdded);
            // remove the added item from the store inventory
            stockInventory.RemoveItem(added.item, added.amountAdded);
        }
    }

    public void RemoveAllItems()
    {
        tierManager.ClearShelf();

        for (int i = 0; i < shelfInventory.Container.Count; i++)
        {
            var slot = shelfInventory.Container[i];
            if (slot.item != null)
            {
                Debug.Log($"Removed {slot.item.name} from slot {i + 1}");
                var removed = shelfInventory.RemoveItem(slot.item, 100);
                // add the items back to store inventory
                stockInventory.AddItem(removed.item, stockInventory.maxCapacity, removed.amount);
            }
        }

        Debug.Log("All items removed from inventory!");
        shelfInventory.PrintInventory();
    }
}
