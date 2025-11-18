using TMPro;
using UnityEngine;

public class ItemValue : MonoBehaviour
{
    InventoryObject inventory;
    
    //public TMP_Text displayText;
    public ItemObject[] idealItems;

    public int baseValue = 1;
    public int multiplier = 5;

    public float totalValue;



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
            totalValue = baseValue * multiplier;
        else
            totalValue = baseValue;
    }

    public ItemObject GetBestIdealItem()
    {
        if (inventory == null || idealItems == null) 
            return null;

        int i = Random.Range(0, idealItems.Length);
        
        return idealItems[i];
    }
}
