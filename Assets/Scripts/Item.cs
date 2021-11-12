using System;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType
    {
        Test
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.Test:
                return ItemAssets.Instance.testSprite;
        }
    }
}
