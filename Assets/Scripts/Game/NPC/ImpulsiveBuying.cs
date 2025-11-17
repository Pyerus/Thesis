using UnityEngine;
using System.Collections.Generic;

public class ImpulsiveBuying
{
    [Header("Impulse Settings")]
    [Range(0f, 1f)]
    public float baseImpulseChance = 1f;      // 5% default
    public float maxImpulseChance = 1f;       // To prevent 100%

    [Header("Cooldown")]
    private float nextAllowedTime = 0f;
    public float impulseCooldown = 1f;

    private Dictionary<ItemObject, float> itemMultipliers = new();


    /// Call this method to check if the NPC decides to buy impulsively.
    public bool TryImpulseBuy(ItemObject itemToBuy)
    {
        if (Time.time < nextAllowedTime)
            return false;

        float multiplier = GetMultiplier(itemToBuy);

        float finalChance = Mathf.Clamp(baseImpulseChance * multiplier, 0f, maxImpulseChance);

        float rng = Random.value;  // 0.0 to 1.0
        bool result = rng < finalChance;

        if (result)
        {
            //buy something
            Debug.Log($"[Impulse Buy Triggered] Chance: {finalChance}. RNG: {rng}");
        }

        nextAllowedTime = Time.time + impulseCooldown;
        return result;
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
