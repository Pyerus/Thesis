using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    public int maxSlots = 4;
    public int maxCapacity = 10;

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

    public (ItemObject item, int amountAdded, int slotIndex) AddItem(ItemObject _item, int maxAmount, int _amount)
    {
        int remaining = _amount;
        int itemsAdded = 0;
        int slotIndex = -1; // -1 means no slot used (inventory full)

        // Try to find existing slot with the same item that has space
        for (int i = 0; i < Container.Count; i++)
        {
            var slot = Container[i];
            if (slot.item == _item && slot.amount < maxAmount)
            {
                int spaceLeft = maxAmount - slot.amount;
                int addAmount = Mathf.Min(remaining, spaceLeft);
                slot.AddAmount(addAmount);
                remaining -= addAmount;
                itemsAdded += addAmount;
                slotIndex = i;

                if (remaining <= 0)
                    return (_item, itemsAdded, slotIndex);
            }
        }

        // Try to fill an empty slot
        for (int i = 0; i < Container.Count; i++)
        {
            var slot = Container[i];
            if (slot.item == null)
            {
                int addAmount = Mathf.Min(remaining, maxAmount);
                slot.item = _item;
                slot.amount = addAmount;
                remaining -= addAmount;
                itemsAdded += addAmount;
                slotIndex = i;

                if (remaining <= 0)
                    return (_item, itemsAdded, slotIndex);
            }
        }

        if (remaining > 0)
        {
            Debug.Log("No empty slots left in inventory!");
        }

        return (_item, itemsAdded, slotIndex);
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

    public int GetItemCount(ItemObject item)
    {
        int total = 0;

        foreach (var slot in Container)
        {
            if (slot.item == item)
            {
                total += slot.amount;
            }
        }

        return total;
    }

    public (ItemObject item, int amount) RemoveItem(ItemObject _item, int _amount)
    {
        foreach (var slot in Container)
        {
            if (slot.item == _item)
            {
                int removedAmount = Mathf.Min(_amount, slot.amount);
                slot.amount -= removedAmount;

                if (slot.amount <= 0)
                {
                    slot.item = null;
                    slot.amount = 0;
                }

                return (_item, removedAmount);
            }
        }

        // Item not found
        return (null, 0);
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
