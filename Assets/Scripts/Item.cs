using System;
using UnityEngine;

[Serializable]
public class Item : ICloneable
{
    public enum ItemType
    {
        Empty,
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
            case ItemType.Empty:
                return ItemAssets.Instance.transparentSprite;
            case ItemType.Test:
                return ItemAssets.Instance.testSprite;
            case ItemType.Apple:
                return ItemAssets.Instance.appleSprite;
        }
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
