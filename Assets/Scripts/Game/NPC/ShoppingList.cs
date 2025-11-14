using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;



public class ShoppingList : MonoBehaviour
{
    [Header("Event Items")]
    public ItemObject[] eventItems;
    
    [Header("Generation Settings")]
    public int minItems = 1;
    public int maxItems = 10;
    public int minQuantity = 1;
    public int maxQuantity = 5;

    [Header("Product Pool (Weighted)")]
    public List<WeightedProduct> products = new List<WeightedProduct>();

    [Header("Generated Output")]
    public List<ProductEntry> generatedList = new List<ProductEntry>();

    [Header("Items in Cart")]
    public List<ProductEntry> cart = new List<ProductEntry>();


    private GameObject[] shelves;
    private GameObject[] waypoints;


    private void Awake()
    {
        GenerateRandomList();
        FindShelves();
        FindWaypoints();
    }



    public void GenerateRandomList()
    {
        generatedList.Clear();

        if (products == null || products.Count == 0)
        {
            Debug.LogWarning("No products assigned!");
            return;
        }

        int itemCount = Random.Range(minItems, maxItems + 1);

        // Temporary pool to avoid duplicates
        List<WeightedProduct> tempPool = new List<WeightedProduct>(products);

        for (int i = 0; i < itemCount; i++)
        {
            if (tempPool.Count == 0)
                break;

            ItemObject randomProduct = GetWeightedRandomProduct(tempPool);
            if (randomProduct == null)
                continue;

            int randomQuantity = Random.Range(minQuantity, maxQuantity + 1);

            generatedList.Add(new ProductEntry(randomProduct, randomQuantity));

            // Remove already picked products
            tempPool.RemoveAll(p => p.product == randomProduct);
        }
    }


    // Picks a product based on weighted probabilities.
    private ItemObject GetWeightedRandomProduct(List<WeightedProduct> pool)
    {
        float totalWeight = 0f;
        foreach (var item in pool)
        {
            totalWeight += Mathf.Max(0f, item.weight);
        }

        if (totalWeight <= 0f)
            return null;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var item in pool)
        {
            cumulative += Mathf.Max(0f, item.weight);
            if (randomValue <= cumulative)
            {
                return item.product;
            }
        }

        return pool[pool.Count - 1].product; // fallback
    }


    // Set a product's weight directly.
    public void SetProductWeight(ItemObject targetProduct, float newWeight)
    {
        foreach (var wp in products)
        {
            if (wp.product == targetProduct)
            {
                wp.weight = Mathf.Max(0f, newWeight);
                Debug.Log($"{wp.product.name} weight set to {wp.weight}");
                return;
            }
        }

        Debug.LogWarning($"Product {targetProduct.name} not found in product pool.");
    }


    public void PrintList()
    {
        foreach (var entry in generatedList)
        {
            Debug.Log($"{entry.product.name} x{entry.quantity}");
        }
    }





    private void FindShelves()
    {
        if (generatedList == null || generatedList.Count == 0)
        {
            Debug.LogWarning("Shopping generatedList is empty — no shelves to find.");
            return;
        }

        shelves = new GameObject[generatedList.Count];

        for (int i = 0; i < generatedList.Count; i++)
        {
            ProductEntry entry = generatedList[i];
            GameObject shelf = FindItemShelf(entry.product);

            if (shelf != null)
            {
                shelves[i] = shelf;
                //Debug.Log($"Shelf {i + 1}: Found for {entry.product.name}");
            }
            else
            {
                shelves[i] = null;
                //Debug.LogWarning($"No shelf found for {entry.product.name}");
            }
        }
    }


    public GameObject[] GetShelves()
    {
        return shelves;
    }


    private GameObject FindItemShelf(ItemObject item)
    {
        // Find all GameObjects tagged as "Shelf"
        GameObject[] allShelves = GameObject.FindGameObjectsWithTag("Shelf");

        foreach (GameObject shelf in allShelves)
        {
            // Try to get the InventoryObject reference from the shelf
            ShelfInventory shelfInventory = shelf.GetComponent<ShelfInventory>();
            if (shelfInventory == null || shelfInventory.inventory == null)
                continue;

            // Check if this shelf's inventory has the item
            if (shelfInventory.inventory.HasItem(item))
            {
                Debug.Log($"Found {item.name} on shelf '{shelf.name}'.");
                return shelf; // Return the shelf itself
            }
        }

        Debug.LogWarning($"No shelf found for item: {item.name}");
        return null;
    }



    private void FindWaypoints()
    {
        // Make sure the waypoints array matches the number of shelves
        waypoints = new GameObject[shelves.Length];

        for (int i = 0; i < shelves.Length; i++)
        {
            if (shelves[i] != null)
            {
                // Find the child named "Waypoint" under the current shelf
                Transform waypointTransform = shelves[i].transform.Find("Waypoint");

                if (waypointTransform != null)
                {
                    waypoints[i] = waypointTransform.gameObject;
                }
                else
                {
                    Debug.LogWarning($"Shelf '{shelves[i].name}' does not have a child named 'Waypoint'.");
                }
            }
            else
            {
                Debug.LogWarning($"Shelf at index {i} is null.");
            }
        }
    }


    public GameObject[] GetWaypoints()
    {
        return waypoints;
    }






    public void AddToCart(ItemObject item, int amount)
    {
        if (item == null || amount == 0)
        {
            Debug.Log($"Shelf ran out of {item.name}.");
            return;
        }

        cart.Add(new ProductEntry(item, amount));
    }
}













[System.Serializable]
public class ProductEntry
{
    public ItemObject product;
    public int quantity;

    public ProductEntry(ItemObject product, int quantity)
    {
        this.product = product;
        this.quantity = quantity;
    }
}

[System.Serializable]
public class WeightedProduct
{
    public ItemObject product;
    [UnityEngine.Range(0f, 1f)] public float weight = 0.5f; // Higher = more likely to appear
}
