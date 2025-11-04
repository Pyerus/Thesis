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
        // Initialize exactly 'maxSlots' empty slots
        if (Container.Count < maxSlots)
        {
            for (int i = Container.Count; i < maxSlots; i++)
            {
                Container.Add(new InventorySlot(null, 0));
            }
        }
        // If there are too many slots (e.g., from editor changes), trim them
        else if (Container.Count > maxSlots)
        {
            Container.RemoveRange(maxSlots, Container.Count - maxSlots);
        }
    }

    public void AddItem(ItemObject _item, int _amount)
    {
        int remaining = _amount;

        // 1️⃣ Try to find existing slot with the same item that has space
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

        // 2️⃣ Try to fill empty slot
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

    // UPDATED RemoveItem Method
    public void RemoveItem(ItemObject _item, int _amount)
    {
        int amountToRemove = _amount;

        // Loop backwards to remove from the "last" stacks first
        for (int i = Container.Count - 1; i >= 0; i--)
        {
            var slot = Container[i];
            if (slot.item == _item)
            {
                int amountInSlot = slot.amount;
                int removeThisSlot = Mathf.Min(amountToRemove, amountInSlot);

                slot.amount -= removeThisSlot;
                amountToRemove -= removeThisSlot;

                if (slot.amount <= 0)
                {
                    slot.item = null;
                    slot.amount = 0;
                }

                if (amountToRemove <= 0)
                {
                    return; // We've removed the full amount
                }
            }
        }
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
        // You can change this max value if you want
        amount = Mathf.Min(amount + value, 9999); 
    }
}