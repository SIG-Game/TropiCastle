using UnityEngine;

public class ItemWorld : Interactable
{
    public static ItemWorld SpawnItemWorld(Vector3 position, Item.ItemType itemType, int amount)
    {
        Transform transform = Instantiate(ItemAssets.Instance.itemWorld, position, Quaternion.identity);

        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.itemType = itemType;
        itemWorld.amount = amount;
        itemWorld.spriteRenderer.sprite = Item.GetSprite(itemType);

        return itemWorld;
    }

    public static void DropItem(Vector3 dropPosition, Item.ItemType itemType, int amount)
    {
        if (itemType == Item.ItemType.Empty)
            return;

        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randomOffset.Normalize();

        ItemWorld itemWorld = SpawnItemWorld(dropPosition + randomOffset, itemType, amount);

        itemWorld.GetComponent<Rigidbody2D>().AddForce(randomOffset * 0.5f, ForceMode2D.Impulse);
    }

    public Item.ItemType itemType;
    public int amount;
    public bool spawnedFromSpawner;
    public ItemSpawner spawner;

    private PlayerController player;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Item.GetSprite(itemType);
        player = GameObject.FindObjectsOfType<PlayerController>()[0];
    }

    public override void Interact()
    {
        Debug.Log("Item interaction with item of type " + itemType);

        if (itemType == Item.ItemType.Campfire)
        {
            Item hotbarItem = player.GetHotbarItem();
            if (hotbarItem.itemType == Item.ItemType.RawCrabMeat)
            {
                Inventory playerInventory = player.GetInventory();
                playerInventory.RemoveItem(hotbarItem);
                playerInventory.AddItem(Item.ItemType.CookedCrabMeat, 1);
            }
        }
    }
}
