using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImpulsiveBuying
{
    [Header("Impulse Settings")]
    [Range(0f, 1f)]
    public float baseImpulseChance = 0.05f;      // 5% default
    public float maxImpulseChance = 0.75f;       // To prevent 100%

    public float nullChance = 0.3f;

    [Header("Cooldown")]
    private float nextAllowedTime = 0f;
    public float impulseCooldown = 1f;

    private Dictionary<ItemObject, float> itemMultipliers = new();


    /// Call this method to check if the NPC decides to buy impulsively.
    public ItemObject TryImpulseBuy(GameObject shelf)
    {
        // cooldown check
        if (Time.time < nextAllowedTime)
            return null;

        // null safety
        if (shelf == null)
            return null;

        ShelfInventory shelfInventory = shelf.GetComponent<ShelfInventory>();
        if (shelfInventory == null)
            return null;

        InventoryObject inventory = shelfInventory.GetInventoryObject();
        if (inventory == null)
            return null;

        // ideal items definition
        ItemObject[] idealItems = shelf.GetComponent<ItemValue>().idealItems;
        float idealItemMultiplier = 5f;

        // items currently on the shelf
        List<ItemObject> shelfItems = inventory.GetItemsInShelf();
        float normalMultiplier = 1f;

        // --- chance to return null ---
        if (Random.value < nullChance)
        {
            nextAllowedTime = Time.time + impulseCooldown;
            return null;
        }

        // --- Build weighted list ---
        List<(ItemObject item, float weight)> weightedItems = new List<(ItemObject, float)>();

        foreach (var item in shelfItems)
        {
            float weight = normalMultiplier;
            if (idealItems.Contains(item))
                weight = idealItemMultiplier;

            weightedItems.Add((item, weight));
        }

        // If shelf is empty
        if (weightedItems.Count == 0)
        {
            nextAllowedTime = Time.time + impulseCooldown;
            return null;
        }

        // --- Weighted random selection ---
        float totalWeight = weightedItems.Sum(w => w.weight);
        float roll = Random.value * totalWeight;

        foreach (var entry in weightedItems)
        {
            if (roll < entry.weight)
            {
                nextAllowedTime = Time.time + impulseCooldown;
                return entry.item;
            }
            roll -= entry.weight;
        }

        // safety fallback (should never happen)
        nextAllowedTime = Time.time + impulseCooldown;
        return null;
    }


    public float GetMultiplier(ItemObject item)
    {
        if (item == null) return 1f;
        if (itemMultipliers.ContainsKey(item) == false)
            itemMultipliers[item] = 1f;

        return itemMultipliers[item];
    }

    public void AddImpulseFactor(ItemObject item, float amount)
    {
        if (item == null) return;

        if (!itemMultipliers.ContainsKey(item))
            itemMultipliers[item] = 1f;

        itemMultipliers[item] += amount;
    }
}
