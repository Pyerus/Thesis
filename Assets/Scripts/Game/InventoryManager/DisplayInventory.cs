using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayInventory : MonoBehaviour
{
    public Cursors cursor;
    public int xSpaceBetweenItems;
    public int ySpaceBetweenItems;
    public int numColumns;
    public GameObject emptySlotPrefab; 
    public InventoryObject inventory;
    private InventoryObject previousInventory;
    private Dictionary<int, GameObject> itemsDisplayed = new Dictionary<int, GameObject>();

    private TierManager.ShelfType currentShelfType;

    void Start()
    {
        CreateDisplay();
    }

    void Update()
    {
        if (cursor.GetShelfInventory() != null)
        {
            var shelfInventoryComponent = cursor.GetShelfInventory(); // Get the component
            var newInventory = shelfInventoryComponent.GetInventoryObject(); // Get the data
            if (newInventory != inventory)
            {
                previousInventory = inventory;
                inventory = newInventory;

                // Get the TierManager from the same object
                TierManager tierManager = shelfInventoryComponent.GetComponent<TierManager>();
                if (tierManager != null)
                {
                    currentShelfType = tierManager.shelfType;
                }
                else
                {
                    currentShelfType = TierManager.ShelfType.Shelf; 
                }

                ClearDisplay();
                CreateDisplay();
            }
            else
            {
                UpdateDisplay();
            }
        }
    }

    public void CreateDisplay()
    {
        // Determine how many slots
        int slotsToDisplay = (currentShelfType == TierManager.ShelfType.Produce) ? 3 : inventory.Container.Count;
        slotsToDisplay = Mathf.Min(slotsToDisplay, inventory.Container.Count);        
        
        for (int i = 0; i < slotsToDisplay; i++)        
        {
            var slot = inventory.Container[i];
            GameObject obj;

            if (slot.item != null)
            {
                obj = Instantiate(slot.item.prefab, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
            }
            else
            {
                obj = Instantiate(emptySlotPrefab, Vector3.zero, Quaternion.identity, transform);
            }

            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            itemsDisplayed.Add(i, obj);
        }
    }

    public void UpdateDisplay()
    {
        
        // Determine how many slots to display
        int slotsToDisplay = (currentShelfType == TierManager.ShelfType.Produce) ? 3 : inventory.Container.Count;
        slotsToDisplay = Mathf.Min(slotsToDisplay, inventory.Container.Count);
        
        
        for (int i = 0; i < slotsToDisplay; i++)
        {
            var slot = inventory.Container[i];

            if (itemsDisplayed.ContainsKey(i))
            {
                GameObject obj = itemsDisplayed[i];
                if (slot.item == null)
                {
                    Destroy(obj);
                    obj = Instantiate(emptySlotPrefab, Vector3.zero, Quaternion.identity, transform);
                }
                else
                {
                    Destroy(obj);
                    obj = Instantiate(slot.item.prefab, Vector3.zero, Quaternion.identity, transform);
                    obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
                }
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                itemsDisplayed[i] = obj;
            }
        }
    }

    public void ClearDisplay()
    {
        foreach (var item in itemsDisplayed.Values)
        {
            Destroy(item);
        }
        itemsDisplayed.Clear();
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(xSpaceBetweenItems * (i % numColumns), (-ySpaceBetweenItems * (i / numColumns)), 0f);
    }
}



