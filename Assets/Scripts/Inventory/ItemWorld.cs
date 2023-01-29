﻿using UnityEngine;

public class ItemWorld : Interactable
{
    [SerializeField] public ItemWithAmount item;

    public PrefabSpawner spawner { get; set; }

    private void Start()
    {
        // Not in Awake because this needs to happen after ItemWorldPrefabInstanceFactory sets item
        GetComponent<SpriteRenderer>().sprite = item.itemData.sprite;
    }

    public override void Interact(PlayerController player)
    {
        Debug.Log("Item interaction with item named " + item.itemData.name);

        if (item.itemData.name == "Campfire")
        {
            ItemWithAmount hotbarItem = player.GetHotbarItem();
            if (hotbarItem.itemData.name == "RawCrabMeat")
            {
                Inventory playerInventory = player.GetInventory();
                playerInventory.RemoveItem(hotbarItem);
                playerInventory.AddItem(Resources.Load<ItemScriptableObject>("Items/CookedCrabMeat"), 1);
            }
        }
    }
}
