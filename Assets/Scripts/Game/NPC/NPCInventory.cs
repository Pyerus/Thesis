using System.Collections.Generic;
using UnityEngine;

public class NPCInventory : MonoBehaviour
{
    public List<DefaultObject> shoppingCart = new List<DefaultObject>();

    public void AddToCart(DefaultObject item)
    {
        shoppingCart.Add(item);
        Debug.Log($"{name} added {item.itemName} to their cart.");
    }

    public void Checkout()
    {
        float total = 0;
        foreach (var item in shoppingCart)
        {
            total += item.sellPrice;
        }

        Debug.Log($"{name} is checking out with {shoppingCart.Count} items worth {total} credits.");
        shoppingCart.Clear(); // Empty cart after purchase
    }
}
