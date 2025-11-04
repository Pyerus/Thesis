using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemObject item;
    public GameObject itemPrefab;
    public ItemSize itemSize;
}

public enum ItemSize
{
    Small,
    Medium,
    Large
}