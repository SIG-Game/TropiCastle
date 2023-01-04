using UnityEngine;

public class ItemWorld : Interactable
{
    public ItemWithAmount item;
    public bool spawnedFromSpawner;
    public ItemSpawner spawner;

    private ItemScriptableObject cookedCrabScriptableObject;

    private void Awake()
    {
        cookedCrabScriptableObject = Resources.Load<ItemScriptableObject>("Items/CookedCrabMeat");
    }

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
                playerInventory.AddItem(cookedCrabScriptableObject, 1);
            }
        }
    }
}
