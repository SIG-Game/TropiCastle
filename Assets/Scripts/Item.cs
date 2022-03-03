﻿using System;
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
        Rock,
        Vine,
        RawCrabMeat,
        Campfire
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
            case ItemType.Vine:
                return ItemAssets.Instance.vineSprite;
            case ItemType.RawCrabMeat:
                return ItemAssets.Instance.rawCrabMeatSprite;
            case ItemType.Campfire:
                return ItemAssets.Instance.campfireSprite;
        }
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        return GetSprite(itemType);
    }
}
