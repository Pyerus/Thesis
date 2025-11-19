using System.Collections.Generic;
using UnityEngine;

public class PurchasedPosters : MonoBehaviour
{
    
    public event System.Action<ItemObject> OnPosterPurchased;
    private ImpulsiveBuying impulsiveBuying;
   
    public List<ItemObject> boughtPosters = new List<ItemObject>();

    private void Start()
    {
        impulsiveBuying = FindFirstObjectByType<ImpulsiveBuying>();
    }

    public void MarkAsPurchased(ItemObject poster)
    {
        if (!boughtPosters.Contains(poster))
        {
            boughtPosters.Add(poster);
            
            OnPosterPurchased?.Invoke(poster);

            if (impulsiveBuying != null)
            {
                Debug.Log($"Poster Count: {boughtPosters.Count}");
                impulsiveBuying.HasPosters = true;
                impulsiveBuying.ReduceNullChance(boughtPosters.Count);
            }
        }
    }

    public bool IsPurchased(ItemObject poster)
    {
        return boughtPosters.Contains(poster);
    }
    
    public void ResetPurchases()
    {
        boughtPosters.Clear();
    }
}