using UnityEngine;

public class StoreInventory : MonoBehaviour
{
    public InventoryObject inventory;

    private void OnApplicationQuit()
    {
        inventory.ClearInventory();
    }
}


