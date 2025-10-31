using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Cursors cursor;
    public InventoryObject inventory;
    

    private void Update()
    {
        if (cursor.GetShelfInventory() != null)
        {
            inventory = cursor.GetShelfInventory().GetInventoryObject();
        }
    }

    public void AddItemToInventory(Item item)
    {
        if (item != null)
        {
            inventory.AddItem(item.item, inventory.maxCapacity);
        }

    }

    public void TakeItemFromSlot(int slotIndex)
{
    if (slotIndex >= 0 && slotIndex < inventory.Container.Count)
    {
        var slot = inventory.Container[slotIndex];
        if (slot.item != null)
        {
            inventory.RemoveItem(slot.item, inventory.maxCapacity);
            Debug.Log($"Removed 1 {slot.item.name} from slot {slotIndex + 1}");
            inventory.PrintInventory();
        }
        else
        {
            Debug.Log($"Slot {slotIndex + 1} is empty!");
        }
    }
}


    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }
}
