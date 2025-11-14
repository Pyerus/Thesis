using UnityEngine;

public class InventoryTabController : MonoBehaviour
{
    [Header("Tab Panels")]
    [SerializeField] private GameObject tabItems;
    [SerializeField] private GameObject tabInfo;

    public void ShowItemsTab()
    {
        tabItems.SetActive(true);
        tabInfo.SetActive(false);
    }

    public void ShowInfoTab()
    {
        tabItems.SetActive(false);
        tabInfo.SetActive(true);
    }

    private void Start()
    {
        ShowItemsTab(); 
    }
}
