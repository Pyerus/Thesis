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

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }
}
