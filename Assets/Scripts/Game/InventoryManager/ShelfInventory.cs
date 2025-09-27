using UnityEngine;

public class ShelfInventory : MonoBehaviour
{
    public InventoryObject inventory;

    public void AddItemToInventory(Item item)
    {
        if (item != null)
        {
            inventory.AddItem(item.item, 1);
        }
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }

    public InventoryObject GetInventoryObject()
    {
        return inventory;
    }
}
