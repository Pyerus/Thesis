using UnityEngine;

public enum ItemType
{
    Food,
    Equipment,
    Default
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemObject : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public GameObject prefab;
    public ItemType type;
    [TextArea(5, 10)] public string description;

    [Header("Economy Settings")]
    public float buyPrice;
    public float sellPrice;
}
