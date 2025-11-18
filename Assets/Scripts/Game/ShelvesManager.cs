using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShelvesManager : MonoBehaviour
{
    GameObject[] shelves;
    List<ShelfInventory> shelfInventories = new List<ShelfInventory>();
    List<InventoryObject> inventoryObjects = new List<InventoryObject>();

    ImpulsiveBuying impulsiveBuying;

    private void Start()
    {
        impulsiveBuying = FindFirstObjectByType<ImpulsiveBuying>();
        shelves = GameObject.FindGameObjectsWithTag("Shelf");
        
        foreach (GameObject shelf in shelves)
        {
            if (shelf.TryGetComponent(out ShelfInventory inv))
            {
                shelfInventories.Add(inv);
            }

            Debug.Log(shelf.name);
        }

        foreach (ShelfInventory shelfInventory in shelfInventories)
        {
            if (shelfInventory == null) return;

            inventoryObjects.Add(shelfInventory.GetInventoryObject());
        }

        Debug.Log($"ShelvesManager: Cached {shelfInventories.Count}/{inventoryObjects.Count} shelves.");
    }

    public GameObject SearchShelvesWithProduct(ItemObject item)
    {
        if (item == null) return null;

        for (int i = 0; i < inventoryObjects.Count; i++)
        {
            if (inventoryObjects[i].HasItem(item))
                return shelfInventories[i].gameObject; // return the GameObject
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



    public float CalculatePromotionScore()
    {
        float total = 0f;

        int noOfShelves = shelfInventories.Count; // number of shelves

        foreach (GameObject shelf in shelves)
        {
            ItemValue itemValue = shelf.GetComponent<ItemValue>();
            total += itemValue.totalValue;
        }

        return total;
    }

    public float CalculateLayoutScore()
    {
        float total = 0f;

        int noOfShelves = shelfInventories.Count;

        foreach (GameObject shelf in shelves)
        {
            ItemValue itemValue = shelf.GetComponent<ItemValue>();
            float visibility = itemValue.totalValue * impulsiveBuying.noOfPosters;
            total += visibility;
        }

        return total;
    }
}
