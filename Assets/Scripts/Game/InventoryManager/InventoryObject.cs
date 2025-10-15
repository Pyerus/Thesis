using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    public int maxSlots = 4;      // total number of slots allowed
    public int maxCapacity = 10;  // max number of items per slot

    public void AddItem(ItemObject _item, int _amount)
    {
        int remaining = _amount;

        // 1️⃣ Fill existing slots first
        foreach (var slot in Container)
        {
            if (slot.item == _item)
            {
                int spaceLeft = maxCapacity - slot.amount;
                if (spaceLeft > 0)
                {
                    int amountToAdd = Mathf.Min(remaining, spaceLeft);
                    slot.AddAmount(amountToAdd);
                    remaining -= amountToAdd;
                }

                if (remaining <= 0)
                    return;
            }
        }

        // 2️⃣ If no existing stack had space, make new slots
        while (remaining > 0)
        {
            if (Container.Count >= maxSlots)
            {
                Debug.Log("Inventory full — no more slots available.");
                return;
            }

            int amountToAdd = Mathf.Min(remaining, maxCapacity);
            Container.Add(new InventorySlot(_item, amountToAdd));
            remaining -= amountToAdd;
        }
    }

    // 🧹 Optional helper for debugging:
    public void PrintInventory()
    {
        for (int i = 0; i < Container.Count; i++)
        {
            Debug.Log($"Slot {i + 1}: {Container[i].item.name} x{Container[i].amount}");
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
        amount = Mathf.Min(amount + value, 9999); // safety cap
    }
}
