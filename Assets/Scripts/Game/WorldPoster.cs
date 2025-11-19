using UnityEngine;

public class WorldPoster : MonoBehaviour
{
    [Header("References")]
   
    public PurchasedPosters purchasedPosters;
    
    
    public ItemObject posterID;
    
 
    public GameObject posterVisuals; 

    void Start()
    {
        purchasedPosters = FindFirstObjectByType<PurchasedPosters>();

        if (purchasedPosters.IsPurchased(posterID))
        {
            posterVisuals.SetActive(true);
        }
        else
        {
            posterVisuals.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (purchasedPosters != null)
        {
            purchasedPosters.OnPosterPurchased += HandlePosterPurchase;
        }
    }

    void OnDisable()
    {
        if (purchasedPosters != null)
        {
            purchasedPosters.OnPosterPurchased -= HandlePosterPurchase;
        }
    }

    void HandlePosterPurchase(ItemObject boughtPoster)
    {
        
        if (boughtPoster == posterID)
        {
            
            posterVisuals.SetActive(true);
        }
    }
}