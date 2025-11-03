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

public void RemoveAllItems()
{
    for (int i = 0; i < inventory.Container.Count; i++)
    {
        var slot = inventory.Container[i];
        if (slot.item != null)
        {
            Debug.Log($"Removed {slot.item.name} from slot {i + 1}");
            inventory.RemoveItem(slot.item, inventory.maxCapacity);
        }
    }

    tierManager.ClearShelf();

    Debug.Log("All items removed from inventory!");
    inventory.PrintInventory();
}

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }
}
