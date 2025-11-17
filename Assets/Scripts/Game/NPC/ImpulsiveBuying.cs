using UnityEngine;

public class ImpulsiveBuying
{
    [Header("Impulse Settings")]
    [Range(0f, 1f)]
    public float baseImpulseChance = 1f;      // 5% default
    public float impulseMultiplier = 1f;         // Affects final chance
    public float maxImpulseChance = 1f;       // To prevent 100%

    [Header("Cooldown")]
    private float nextAllowedTime = 0f;
    public float impulseCooldown = 1f;

    /// Call this method to check if the NPC decides to buy impulsively.
    public bool TryImpulseBuy()
    {
        if (Time.time < nextAllowedTime)
            return false;

        float finalChance = Mathf.Clamp(baseImpulseChance * impulseMultiplier, 0f, maxImpulseChance);

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

    public void AddImpulseFactor(float amount)
    {
        impulseMultiplier += amount;
    }

    /// <summary>
    /// Optional – reset multiplier after action.
    /// </summary>
    public void ResetMultiplier()
    {
        impulseMultiplier = 1f;
    }
}
