using UnityEngine;

public class ShelfInventory : MonoBehaviour
{
    public InventoryObject inventory;
    private TierManager tierManager;

    private void Start()
    {
        tierManager = GetComponent<TierManager>();
    }

    private void OnApplicationQuit()
    {
        //inventory.Container.Clear();
    }

    public InventoryObject GetInventoryObject()
    {
        return inventory;
    }

    public TierManager GetTierManager()
    {
        return tierManager;
    }
}
