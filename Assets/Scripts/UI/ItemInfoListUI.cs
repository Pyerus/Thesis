using UnityEngine;
using TMPro; 

public class ItemInfoListUI : MonoBehaviour
{
    
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI buyPriceText;
    public TextMeshProUGUI sellPriceText;

   
    public void Setup(ItemObject item)
    {
        nameText.text = item.itemName;
        buyPriceText.text = $"₱{item.buyPrice}";
        sellPriceText.text = $"₱{item.sellPrice}";
    }
}