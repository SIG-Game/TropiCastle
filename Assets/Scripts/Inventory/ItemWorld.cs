using UnityEngine;

public class ItemWorld : Interactable
{
    public static void SpawnItemWorld(GameObject itemWorldPrefab, Vector3 position, ItemWithAmount itemToSpawn)
    {
        GameObject spawnedGameObject = Instantiate(itemWorldPrefab, position, Quaternion.identity);

        ItemWorld itemWorld = spawnedGameObject.GetComponent<ItemWorld>();
        itemWorld.item = itemToSpawn;
        itemWorld.spriteRenderer.sprite = itemToSpawn.itemData.sprite;
    }

    public static void DropItem(GameObject itemWorldPrefab, Vector3 dropPosition, ItemWithAmount itemToDrop)
    {
        if (itemToDrop.itemData.name == "Empty")
            return;

        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randomOffset.Normalize();
        randomOffset *= 0.7f;

        SpawnItemWorld(itemWorldPrefab, dropPosition + randomOffset, itemToDrop);

        // Dropped items currently don't move
        //itemWorld.GetComponent<Rigidbody2D>().AddForce(randomOffset * 0.5f, ForceMode2D.Impulse);
    }

    public ItemWithAmount item;
    public bool spawnedFromSpawner;
    public ItemSpawner spawner;

    private PlayerController player;
    private SpriteRenderer spriteRenderer;
    private ItemScriptableObject cookedCrabScriptableObject;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.itemData.sprite;
        player = FindObjectsOfType<PlayerController>()[0];
        cookedCrabScriptableObject = Resources.Load<ItemScriptableObject>("Items/CookedCrabMeat");
    }

    public override void Interact()
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
