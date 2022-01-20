using System;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType
    {
        Empty,
        Test,
        Apple,
        Stick,
        Spear,
        Rock
    }

    public static Sprite GetSprite(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case ItemType.Empty:
                return ItemAssets.Instance.transparentSprite;
            case ItemType.Test:
                return ItemAssets.Instance.testSprite;
            case ItemType.Apple:
                return ItemAssets.Instance.appleSprite;
            case ItemType.Stick:
                return ItemAssets.Instance.stickSprite;
            case ItemType.Spear:
                return ItemAssets.Instance.spearSprite;
            case ItemType.Rock:
                return ItemAssets.Instance.rockSprite;
        }
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        return GetSprite(itemType);
    }
}
