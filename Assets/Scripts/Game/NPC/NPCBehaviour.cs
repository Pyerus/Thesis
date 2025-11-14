using System.Collections.Generic;
using UnityEngine;

// This script connects everything together.
public class NPCBehaviour : MonoBehaviour
{
    ShoppingList shoppingList;
    MoneyCounter moneyCounter;

    private void Start()
    {
        shoppingList = gameObject.GetComponent<ShoppingList>();

        moneyCounter = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MoneyCounter>();
    }



    // When the NPC reaches the target waypoint (shelf), remove the product from the shelf and add to cart.
    public void AddToCart(ShelfInventory shelfInventory, ItemObject itemObject, int amount)
    {
        // Take the shelf components associated with the waypoint.
        InventoryObject inventory = shelfInventory.GetInventoryObject();
        TierManager tierManager = shelfInventory.GetTierManager();

        
        // Remove product from inventory and take return variables (itemObject, amount, slotIndex).
        var product = inventory.RemoveItem(itemObject, amount);
        
        // Get the item based on index.
        Item item = tierManager.GetItemFromIndex(product.slotIndex);

        // Take the returned amount from the shelf.
        tierManager.TakeItemsFromShelf(product.slotIndex, product.amount);


        // Add the removed items to cart.
        shoppingList.AddToCart(itemObject, product.amount);
    }



    // When the NPC reaches the counter, check out.
    public void CheckOut()
    {
        Debug.Log("Checking out.");

        if (shoppingList.cart == null || shoppingList.cart.Count == 0)
        {
            Debug.Log("No product bought.");
            return;
        }

        float totalCost = 0;

        foreach (var entry in shoppingList.cart)
        {
            Debug.Log($"{entry.quantity}x {entry.product.itemName}: ${entry.product.sellPrice} each.");
            totalCost += entry.product.sellPrice * entry.quantity;
        }

        Debug.Log($"Bought {shoppingList.cart.Count} products for ${totalCost}");
        moneyCounter.AddMoney(totalCost);
    }
}
