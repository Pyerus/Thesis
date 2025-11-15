using Unity.VisualScripting;
using UnityEngine;

public class StoreInventory : MonoBehaviour
{
    public InventoryObject inventory;
    public int defaultQuantity = 0;

    private void Start()
    {
        foreach (var slot in inventory.Container)
        {
            slot.amount = defaultQuantity;
        }
    }

    private void OnApplicationQuit()
    {
        inventory.ClearInventory();
    }
}


