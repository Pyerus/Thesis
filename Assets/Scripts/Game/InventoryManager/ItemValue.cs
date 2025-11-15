using TMPro;
using UnityEngine;

public class ItemValue : MonoBehaviour
{
    InventoryObject inventory;
    int totalValue = 0;
    
    //public TMP_Text displayText;
    public ItemObject[] idealItems;
    public int baseValue = 10;
    public int additionalValue = 10;



    private void Start()
    {
        inventory = GetComponent<ShelfInventory>().GetInventoryObject();
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
            totalValue = baseValue + additionalValue;
        else
            totalValue = baseValue;
    }
}
