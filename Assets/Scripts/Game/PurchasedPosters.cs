using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PurchasedPosters", menuName = "Inventory/Purchased Posters List")]
public class PurchasedPosters : ScriptableObject
{
    
    public event System.Action<ItemObject> OnPosterPurchased;
    
   
    public List<ItemObject> boughtPosters = new List<ItemObject>();

    public void MarkAsPurchased(ItemObject poster)
    {
        if (!boughtPosters.Contains(poster))
        {
            boughtPosters.Add(poster);
            
            OnPosterPurchased?.Invoke(poster);
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