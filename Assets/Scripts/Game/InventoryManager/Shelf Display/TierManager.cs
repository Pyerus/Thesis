using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class TierManager : MonoBehaviour
{
    [Header("Shelf Tier")] 
    public GameObject tier1;
    public GameObject tier2;
    public GameObject tier3;
    public GameObject tier4;

    [Header("Item Prefabs")]
    public GameObject tier1Item;
    public GameObject tier2Item;
    public GameObject tier3Item;
    public GameObject tier4Item;

    // slot arrangement
    GameObject[] tier1Slots;
    GameObject[] tier2Slots;
    GameObject[] tier3Slots;
    GameObject[] tier4Slots;



    public void StockItems(Item item) // method called outside to initialize stocking
    {
        // check tier level and set prefab (default to tier 1 for now)
        tier1Item = item.itemPrefab;

        // check item size and decide slot arrangement
        string itemSize = CheckItemSize(item);

        // get slots
        tier1Slots = SetTierArrangement(tier1, itemSize);

        // place items on the shelf
        PlaceItems(tier1Slots, tier1Item);
    }

    public void ClearShelf()
    {
        RemoveAllItems(tier1Slots);
    }



    private string CheckItemSize(Item item)
    {
        // check item size to decide slot arrangement
        string size = "";

        if (item.itemSize == ItemSize.Small)
        {
            size = "Small Items";
        }
        else if (item.itemSize == ItemSize.Medium)
        {
            size = "Medium Items";
        }
        else if (item.itemSize == ItemSize.Large)
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
}
