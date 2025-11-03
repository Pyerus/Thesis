using UnityEngine;

public class ItemCategory : MonoBehaviour
{
    public enum Category
    {
        Big,
        Medium,
        Small
    }

    [Header("Select the category for this prefab")]
    public Category category;
}
