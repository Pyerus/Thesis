using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImpulsiveBuying : MonoBehaviour
{
    [Header("Impulse Settings")]
    [Range(0f, 1f)]
    public float maxImpulseChance = 0.75f;       // To prevent 100%
    public bool HasPosters = false;
    public float nullChance = 0.3f;

    [Header("Cooldown")]
    private float nextAllowedTime = 0f;
    public float impulseCooldown = 1f;

    private Dictionary<ItemObject, float> itemMultipliers = new();

    public int noOfPosters = 0;


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
        float idealItemMultiplier = shelf.GetComponent<ItemValue>().multiplier;

        // items currently on the shelf
        List<ItemObject> shelfItems = inventory.GetItemsInShelf();
        float normalMultiplier = shelf.GetComponent<ItemValue>().baseValue;

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

    public void ReduceNullChance(int postersBought)
    {
        if (!HasPosters)
            return;

        noOfPosters = postersBought;

        // Base start value
        float baseNullChance = 0.30f;

        // Reduce by 0.05 for each purchased poster
        nullChance = baseNullChance - (postersBought * 0.05f);

        // Clamp so that we never go below 0.05
        nullChance = Mathf.Clamp(nullChance, 0.05f, baseNullChance);

        Debug.Log($"[Impulse] Posters bought: {postersBought} | nullChance is now {nullChance}");
    }
}
