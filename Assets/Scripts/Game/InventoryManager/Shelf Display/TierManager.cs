using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class TierManager : MonoBehaviour
{
    public ShelfType shelfType; // this doesn't do anything lol
    
    [Header("Shelf Tier")] 
    public GameObject tier1;
    public GameObject tier2;
    public GameObject tier3;
    public GameObject tier4;

    [Header("Slot Arrangements")]
    GameObject[] tier1Slots;
    GameObject[] tier2Slots;
    GameObject[] tier3Slots;
    GameObject[] tier4Slots;



    public void StockItems(Item item, int tierIndex)
    {
        // Declare variables to hold the references
        GameObject tier;
        GameObject[] tierSlots;

        // Check tier level and assign slots
        CheckTier(tierIndex, out tier);

        Debug.Log("StockItems() invoked. Index: " + tierIndex.ToString() + ". Tier: " + tier);

        if (tier != null)
        {
            // check item size and decide slot arrangement
            string itemSize = CheckItemSize(item.itemPrefab);

            // set item arrangement based on size
            tierSlots = SetTierArrangement(tier, itemSize);

            // place items on the shelf
            PlaceItems(tierSlots, item.itemPrefab);

            // Update the tier array after modification
            if (tierIndex == 0)
                tier1Slots = tierSlots;
            else if (tierIndex == 1)
                tier2Slots = tierSlots;
            else if (tierIndex == 2)
                tier3Slots = tierSlots;
            else if (tierIndex == 3)
                tier4Slots = tierSlots;
        }
    }

    public void ClearShelf()
    {
        Debug.Log("ClearShelf() invoked.");
        RemoveAllItems(tier1Slots);
        RemoveAllItems(tier2Slots);
        RemoveAllItems(tier3Slots);
        RemoveAllItems(tier4Slots);
    }



    private void CheckTier(int tierIndex, out GameObject tier)
    {
        tier = tierIndex switch
        {
            0 => tier1,
            1 => tier2,
            2 => tier3,
            3 => tier4,
            _ => null
        };
    }

    private string CheckItemSize(GameObject itemPrefab)
    {
        // check item size to decide slot arrangement
        ItemCategory category = itemPrefab.GetComponent<ItemCategory>();
        string size = "";

        if (category.category == ItemCategory.Category.Small)
        {
            size = "Small Items";
        }
        else if (category.category == ItemCategory.Category.Medium)
        {
            size = "Medium Items";
        }
        else if (category.category == ItemCategory.Category.Big)
        {
            size = "Large Items";
        }

        return size;
    }

    private GameObject[] SetTierArrangement(GameObject tier, string itemSize)
    {
        // set type based on item size
        GameObject[] tierSlots = new GameObject[tier.transform.Find(itemSize).childCount];

        for (int i = 0; i < tier.transform.Find(itemSize).childCount; i++)
        {
            tierSlots[i] = tier.transform.Find(itemSize).GetChild(i).gameObject;
        }

        return tierSlots;
    }

    private void PlaceItems(GameObject[] slots, GameObject item)
    {
        foreach (GameObject slot in slots)
        {
            if (slot.transform.childCount == 0)
            {
                GameObject newItem = Instantiate(item, slot.transform);
            }
        }
    }

    private void RemoveLastItem(GameObject[] slots)
    {
        if (slots != null)
        {
            for (int i = slots.Length - 1; i >= 0; i--)
            {
                if (slots[i].transform.childCount != 0)
                {
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                    break;
                }
            }
        }
    }

    private void RemoveAllItems(GameObject[] slots)
    {
        if (slots != null)
        {
            for (int i = slots.Length - 1; i >= 0; i--)
            {
                if (slots[i].transform.childCount != 0)
                {
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                }
            }
        }
    }

    public enum ShelfType
    {
        Shelf,
        Fridge,
        Produce
    }
}
