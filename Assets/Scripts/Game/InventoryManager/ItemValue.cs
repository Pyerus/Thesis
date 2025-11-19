using TMPro;
using UnityEngine;

public class ItemValue : MonoBehaviour
{
    InventoryObject inventory;
    ImpulsiveBuying impulsiveBuying;

    public Waypoint waypoint;
    public ItemObject[] idealItems;

    public float baseValue = 10;
    public float multiplier = 2;

    public float totalValue;



    private void Start()
    {
        inventory = GetComponent<ShelfInventory>().GetInventoryObject();
        impulsiveBuying = FindFirstObjectByType<ImpulsiveBuying>();
    }

    private void Update()
    {
        UpdateShelfValue();
        //displayText.text = totalValue.ToString();
    }

    public void UpdateShelfValue()
    {
        if (inventory == null || idealItems == null || idealItems.Length == 0)
        {
            return;
        }
        
        bool hasAnyIdealItem = false;
        float posterMultiplier = impulsiveBuying.posterMultiplier;

        // Check if inventory contains ANY ideal item
        foreach (var item in idealItems)
        {
            if (item != null && inventory.HasItem(item))
            {
                hasAnyIdealItem = true;
                break;
            }
        }

        if (hasAnyIdealItem)
        {
            waypoint.IncreaseVisibility();
            totalValue = baseValue * multiplier * posterMultiplier;
        }
        else
        {
            waypoint.SetDefaultVisibility();
            totalValue = baseValue * posterMultiplier;
        }
    }

    public ItemObject GetBestIdealItem()
    {
        if (inventory == null || idealItems == null) 
            return null;

        int i = Random.Range(0, idealItems.Length);
        
        return idealItems[i];
    }

    public float GetItemValue(ItemObject givenItem)
    {
        if (givenItem == null) return 0f;

        // Check if item is an ideal item
        foreach (var item in idealItems)
        {
            if (item == null) break;

            if (item == givenItem)
            {
                return baseValue * multiplier;
            }
        }

        return baseValue;
    }
}
