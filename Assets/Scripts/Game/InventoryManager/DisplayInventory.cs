using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DisplayInventory : MonoBehaviour
{
    public Cursors cursor;
    public int xSpaceBetweenItems; //Horizontal Space between items
    public int ySpaceBetweenItems; //Vertical Space between items
    public int numColumns; //Number of columns

    public InventoryObject inventory; //Interchangeable
    private InventoryObject previousInventory;

    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();
    void Start()
    {
        CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (cursor.GetShelfInventory() != null)
        {
            var newInventory = cursor.GetShelfInventory().GetInventoryObject();

            if (newInventory != inventory)
            {
                previousInventory = inventory;
                inventory = newInventory;

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
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            var obj = Instantiate(inventory.Container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
            itemsDisplayed.Add(inventory.Container[i], obj);

        }

    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            if (itemsDisplayed.ContainsKey(inventory.Container[i]))
            {
                itemsDisplayed[inventory.Container[i]].GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
            }
            else
            {
                var obj = Instantiate(inventory.Container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
                itemsDisplayed.Add(inventory.Container[i], obj);
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
