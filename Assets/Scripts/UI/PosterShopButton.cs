using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PosterShopButton : MonoBehaviour
{
    
    public PurchasedPosters purchasedPosters;
    
    
    public ItemObject posterID;
    
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }
    
    void OnEnable()
    {
        purchasedPosters = FindFirstObjectByType<PurchasedPosters>();

        if (purchasedPosters != null)
        {
            purchasedPosters.OnPosterPurchased += HandlePosterPurchase;

            CheckIfPurchased();
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
            button.interactable = false;
        }
    }

    
    void CheckIfPurchased()
    {
        if (purchasedPosters.IsPurchased(posterID))
        {
            button.interactable = false;
        }
    }
}