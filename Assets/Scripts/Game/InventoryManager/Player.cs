using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
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
}
