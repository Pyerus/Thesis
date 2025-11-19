using UnityEngine;

public class PosterReset : MonoBehaviour
{
    
    public PurchasedPosters postersToReset;

    
    void Awake()
    {
        postersToReset = FindFirstObjectByType<PurchasedPosters>();

        #if UNITY_EDITOR
        if (postersToReset != null)
        {
            postersToReset.ResetPurchases();
        }
        else
        {
            Debug.LogError("GameResetter: 'Posters To Reset' not assigned in Inspector!");
        }
        #endif
    }
}