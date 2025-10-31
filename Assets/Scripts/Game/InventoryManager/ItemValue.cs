using TMPro;
using UnityEngine;

public class ItemValue : MonoBehaviour
{
    InventoryObject inventory;
    int totalValue = 0;
    
    //public TMP_Text displayText;
    public ItemObject idealItem;
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
        if (inventory == null || idealItem == null)
        {
            return;
        }
        
        if (inventory.HasItem(idealItem))
        {
            totalValue = baseValue + additionalValue;
        }
        else
        {
            totalValue = baseValue;
        }

        Debug.Log(totalValue);
    }
}
