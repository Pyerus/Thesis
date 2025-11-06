using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    public int maxSlots = 4;
    public int maxCapacity = 10;
    public int currentStock;
    public int maxStock = 99;

    private void OnEnable()
    {
        // Initialize exactly 4 empty slots
        if (Container.Count < maxSlots)
        {
            for (int i = Container.Count; i < maxSlots; i++)
            {
                Container.Add(new InventorySlot(null, 0));
            }
        }
    }

    public void AddStock()
    {
        //If the current stock exceeds it will return nothing and will 
        //just print out 
        //"Cannot Add more to Stock"
        if (currentStock > maxStock)
        {
            Debug.Log("Max Stock Cannot Add More");
        }
    }
    
    public void StockToSlot()
    {
        //Decrease Current Stock present when adding to slot
    }

    public void AddItem(ItemObject _item, int _amount)
    {
        int remaining = _amount;

        //Try to find existing slot with the same item that has space
        foreach (var slot in Container)
        {
            if (slot.item == _item && slot.amount < maxCapacity)
            {
                int spaceLeft = maxCapacity - slot.amount;
                int addAmount = Mathf.Min(remaining, spaceLeft);
                slot.AddAmount(addAmount);
                remaining -= addAmount;

                if (remaining <= 0)
                    return;
            }
        }

        //Try to fill empty slot
        foreach (var slot in Container)
        {
            if (slot.item == null)
            {
                int addAmount = Mathf.Min(remaining, maxCapacity);
                slot.item = _item;
                slot.amount = addAmount;
                remaining -= addAmount;

                if (remaining <= 0)
                    return;
            }
        }

        if (remaining > 0)
        {
            Debug.Log("No empty slots left in inventory!");
        }
    }

    // Check if inventory has an item (used for probability calculation)
    public bool HasItem(ItemObject item)
    {
        foreach (var slot in Container)
        {
            if (slot.item == item)
                return true;
        }
        return false;
    }

    public void RemoveItem(ItemObject _item, int _amount)
    {
        foreach (var slot in Container)
        {
            if (slot.item == _item)
            {
                slot.amount -= _amount;
                if (slot.amount <= 0)
                {
                    slot.item = null;
                    slot.amount = 0;
                }
                return;
            }
        }

        //Create method for bringing back item to stock after removing
    }

    public void PrintInventory()
    {
        for (int i = 0; i < Container.Count; i++)
        {
            string itemName = Container[i].item != null ? Container[i].item.name : "Empty";
            Debug.Log($"Slot {i + 1}: {itemName} x{Container[i].amount}");
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;

    public InventorySlot(ItemObject _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount = Mathf.Min(amount + value, 9999);
    }
}
