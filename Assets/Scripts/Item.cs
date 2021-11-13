using System;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType
    {
        Test,
        Apple
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
            case ItemType.Apple:
                return ItemAssets.Instance.appleSprite;
        }
    }
}
