using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class TierManager : MonoBehaviour
{
    [Header("Shelf Type")]
    public ShelfType shelfType;

    [Header("Shelf Tiers")]
    public GameObject tier1;
    public GameObject tier2;
    public GameObject tier3;
    public GameObject tier4;

    [Header("Slot Arrangements (auto-filled)")]
    private GameObject[] tier1Slots;
    private GameObject[] tier2Slots;
    private GameObject[] tier3Slots;
    private GameObject[] tier4Slots;

    [Header("Items assigned to shelf tiers.")]
    private Item tier1Item;
    private Item tier2Item;
    private Item tier3Item;
    private Item tier4Item;



    /// Stocks items on a given tier of the shelf.
    public void StockItems(Item item, int tierIndex, int amount)
    {
        if (item == null || item.itemPrefab == null)
        {
            Debug.LogError("Item or its prefab is null. Cannot stock items.");
            return;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("Amount must be greater than zero.");
            return;
        }

        // Select the correct tier
        GameObject tier = GetTierByIndex(tierIndex);
        if (tier == null)
        {
            Debug.LogWarning($"Invalid tier index: {tierIndex}");
            return;
        }

        // Set item tier
        SetItemOnIndex(item, tierIndex);

        // Determine size and slot group
        string itemSize = CheckItemSize(item.itemPrefab);
        GameObject[] tierSlots = SetTierArrangement(tier, itemSize);

        if (tierSlots == null || tierSlots.Length == 0)
        {
            Debug.LogWarning($"No slots found for size '{itemSize}' on tier {tierIndex}");
            return;
        }

        // Place items
        PlaceItems(tierSlots, item.itemPrefab, amount);

        // Cache tier slots for later reference
        switch (tierIndex)
        {
            case 0: tier1Slots = tierSlots; break;
            case 1: tier2Slots = tierSlots; break;
            case 2: tier3Slots = tierSlots; break;
            case 3: tier4Slots = tierSlots; break;
        }

        Debug.Log($"Stocked {amount}x '{item.name}' on tier {tierIndex} ({itemSize})");
    }


    /// Determines how many of an item can fit on this shelf type.
    public int GetMaxAmount(Item item)
    {
        if (item?.itemPrefab == null)
            return 0;

        var category = item.itemPrefab.GetComponent<ItemCategory>();
        if (category == null)
            return 0;

        switch (category.category)
        {
            case ItemCategory.Category.Small:
                return shelfType == ShelfType.Fridge ? 16 : 18;

            case ItemCategory.Category.Medium:
                return shelfType switch
                {
                    ShelfType.Shelf => 10,
                    ShelfType.Fridge => 8,
                    _ => 0
                };

            case ItemCategory.Category.Big:
                return shelfType switch
                {
                    ShelfType.Shelf => 10,
                    ShelfType.Fridge => 4,
                    _ => 0
                };

            default:
                return 0;
        }
    }


    public Item GetItemFromIndex(int index)
    {
        switch (index)
        {
            case 0: return tier1Item;
            case 1: return tier2Item;
            case 2: return tier3Item;
            case 3: return tier4Item;
        }

        return null;
    }


    /// Clears all items from the shelf.
    public void ClearShelf()
    {
        Debug.Log("Clearing all shelf tiers...");

        RemoveAllItems(tier1Slots);
        RemoveAllItems(tier2Slots);
        RemoveAllItems(tier3Slots);
        RemoveAllItems(tier4Slots);

        tier1Item = null;
        tier2Item = null;
        tier3Item = null;
        tier4Item = null;
    }

    public void TakeItemsFromShelf(int index, int amount)
    {
        switch (index)
        {
            case 0: RemoveLastItems(tier1Slots, amount); break;
            case 1: RemoveLastItems(tier2Slots, amount); break;
            case 2: RemoveLastItems(tier3Slots, amount); break;
            case 3: RemoveLastItems(tier4Slots, amount); break;
        }
    }





    // ---------- PRIVATE HELPERS ----------

    private GameObject GetTierByIndex(int tierIndex)
    {
        return tierIndex switch
        {
            0 => tier1,
            1 => tier2,
            2 => tier3,
            3 => tier4,
            _ => null
        };
    }

    private void SetItemOnIndex(Item item, int index)
    {
        switch (index)
        {
            case 0: tier1Item = item; break;
            case 1: tier2Item = item; break;
            case 2: tier3Item = item; break;
            case 3: tier4Item = item; break;
        }
    }

    private string CheckItemSize(GameObject itemPrefab)
    {
        ItemCategory category = itemPrefab.GetComponent<ItemCategory>();
        if (category == null)
        {
            Debug.LogWarning($"Item '{itemPrefab.name}' has no ItemCategory component!");
            return "Small Items";
        }

        return category.category switch
        {
            ItemCategory.Category.Small => "Small Items",
            ItemCategory.Category.Medium => "Medium Items",
            ItemCategory.Category.Big => "Large Items",
            _ => "Small Items"
        };
    }

    private GameObject[] SetTierArrangement(GameObject tier, string itemSize)
    {
        Transform sizeGroup = tier.transform.Find(itemSize);
        if (sizeGroup == null)
        {
            Debug.LogWarning($"Tier '{tier.name}' has no child group '{itemSize}'.");
            return null;
        }

        int slotCount = sizeGroup.childCount;
        GameObject[] slots = new GameObject[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            slots[i] = sizeGroup.GetChild(i).gameObject;
        }

        return slots;
    }

    private void PlaceItems(GameObject[] slots, GameObject itemPrefab, int amount)
    {
        int placed = 0;

        foreach (GameObject slot in slots)
        {
            if (slot == null)
                continue;

            if (slot.transform.childCount == 0)
            {
                Instantiate(itemPrefab, slot.transform);
                placed++;

                if (placed >= amount)
                    break;
            }
        }

        if (placed < amount)
        {
            Debug.LogWarning($"Only placed {placed}/{amount} items — not enough empty slots!");
        }
    }

    private void RemoveAllItems(GameObject[] slots)
    {
        if (slots == null)
            return;

        foreach (GameObject slot in slots)
        {
            if (slot != null && slot.transform.childCount > 0)
            {
                for (int i = slot.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(slot.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    private void RemoveLastItems(GameObject[] slots, int amountToRemove)
    {
        if (slots == null || amountToRemove <= 0)
            return;

        int removed = 0;

        // Loop backwards through the slots
        for (int i = slots.Length - 1; i >= 0 && removed < amountToRemove; i--)
        {
            GameObject slot = slots[i];
            if (slot == null)
                continue;

            // Remove the last child (if present)
            if (slot.transform.childCount > 0)
            {
                // Remove only one item per slot
                Destroy(slot.transform.GetChild(slot.transform.childCount - 1).gameObject);
                removed++;
            }
        }

        if (removed < amountToRemove)
            Debug.LogWarning($"Only removed {removed}/{amountToRemove} items — not enough stocked items.");
    }

    public enum ShelfType
    {
        Shelf,
        Fridge,
        Produce
    }
}
