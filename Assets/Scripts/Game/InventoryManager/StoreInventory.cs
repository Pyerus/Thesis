using Unity.VisualScripting;
using UnityEngine;

public class StoreInventory : MonoBehaviour
{
    public InventoryObject inventory;

    private void Start()
    {
        foreach (var slot in inventory.Container)
        {
            slot.amount = 50;
        }
    }

    private void OnApplicationQuit()
    {
        inventory.ClearInventory();
    }
}


