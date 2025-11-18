using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PurchasedPosters", menuName = "Inventory/Purchased Posters List")]
public class PurchasedPosters : ScriptableObject
{
    
    public event System.Action<ItemObject> OnPosterPurchased;
    private ImpulsiveBuying impulsiveBuying;
   
    public List<ItemObject> boughtPosters = new List<ItemObject>();

    private void OnEnable()
    {
        if (impulsiveBuying == null)
            impulsiveBuying = new ImpulsiveBuying();
    }

    public void MarkAsPurchased(ItemObject poster)
    {
        if (!boughtPosters.Contains(poster))
        {
            boughtPosters.Add(poster);
            
            OnPosterPurchased?.Invoke(poster);

            if (impulsiveBuying != null)
            {
                Debug.Log("In MarkAsPurchased impulsiveBuying IF");
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