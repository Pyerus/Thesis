using System.Collections.Generic;
using UnityEngine;

public class InventoryInfoDisplay : MonoBehaviour
{
    
    public List<ItemObject> allStoreItems;

   
    public GameObject itemInfoRowPrefab;

    
    public Transform contentContainer;

    void Start()
    {
        PopulateInfoTab();
    }

    void PopulateInfoTab()
    {
        
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        
        foreach (ItemObject item in allStoreItems)
        {
            
            GameObject rowInstance = Instantiate(itemInfoRowPrefab, contentContainer);

            
            ItemInfoListUI rowUI = rowInstance.GetComponent<ItemInfoListUI>();

           
            rowUI.Setup(item);
        }
    }
}