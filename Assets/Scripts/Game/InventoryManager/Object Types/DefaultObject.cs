using UnityEngine;

[CreateAssetMenu(fileName = "New Default Object", menuName = "Inventory System/Items/Default")]
public class DefaultObject : ItemObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        type = ItemType.Default;
    }
}
