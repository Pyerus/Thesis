using UnityEngine;

public enum ItemType
{
    Food,
    Equipment,
    Default
}
public class ItemObject : ScriptableObject
{
    public float price;
    public GameObject prefab;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;

}
