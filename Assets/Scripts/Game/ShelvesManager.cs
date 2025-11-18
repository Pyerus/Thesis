using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShelvesManager : MonoBehaviour
{
    List<ShelfInventory> shelfInventories = new List<ShelfInventory>();
    List<InventoryObject> inventoryObjects = new List<InventoryObject>();

    private void Start()
    {
        foreach (GameObject shelf in GameObject.FindGameObjectsWithTag("Shelf"))
        {
            if (shelf.TryGetComponent(out ShelfInventory inv))
            {
                shelfInventories.Add(inv);
            }
        }

        foreach (ShelfInventory shelfInventory in shelfInventories)
        {
            if (shelfInventory == null) return;

            inventoryObjects.Add(shelfInventory.GetInventoryObject());
        }

        Debug.Log($"ShelvesManager: Cached {shelfInventories.Count}/{inventoryObjects.Count} shelves.");
    }

    public ShelfInventory SearchShelvesForProduct(ItemObject item)
    {
        if (item == null) return null;

        for (int i = 0; i < inventoryObjects.Count; i++)
        {
            if (inventoryObjects[i].HasItem(item))
                return shelfInventories[i];
        }

        return null;
    }

    public GameObject GetRandomShelfGOInRadius(Vector3 fromPosition, float radius)
    {
        List<GameObject> shelvesInRange = new List<GameObject>();

        foreach (ShelfInventory shelf in shelfInventories)
        {
            if (shelf == null) continue;

            float dist = Vector3.Distance(fromPosition, shelf.transform.position);

            if (dist <= radius)
            {
                shelvesInRange.Add(shelf.gameObject);
            }
        }

        if (shelvesInRange.Count == 0)
            return null;

        Debug.Log($"Random shelf found.");

        return shelvesInRange[Random.Range(0, shelvesInRange.Count)];
    }

}
